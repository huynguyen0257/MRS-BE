using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MRS.Model;
using MRS.Service;
using MRS.Utils;
using MRS.ViewModels;

namespace MRS.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _OrderService;
        private readonly ICartService _cartService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly UserManager<User> _userManager;

        public OrderController(IOrderService orderService, ICartService cartService, IOrderDetailService orderDetailService, UserManager<User> userManager)
        {
            _OrderService = orderService;
            this._cartService = cartService;
            _orderDetailService = orderDetailService;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet("MySelf")]
        public ActionResult GetMySelf(int index = 1, int pageSize = 5)
        {
            try
            {
                var user = _userManager.GetUserAsync(User).Result;
                var orders = _OrderService.GetOrders(_ => _.Carts.Select(c => c.UserId).Contains(user.Id));
                return Ok(orders.ToPageList<OrderVM, Order>(index, pageSize));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("All")]
        public ActionResult GetAll(bool IsConfirmed = false, int index = 1, int pageSize = 5)
        {
            try
            {
                if (IsConfirmed)
                {
                    return Ok(_OrderService.GetOrders(_ => _.Status != (int)OrderStatus.processing).ToPageList<OrderVM,Order>(index,pageSize));
                }
                else
                {
                    return Ok(_OrderService.GetOrders(_ => _.Status == (int)OrderStatus.processing).ToPageList<OrderVM, Order>(index, pageSize));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public ActionResult Get(Guid id)
        {
            var Order = _OrderService.GetOrder(id);
            if (Order != null)
            {
                var result = Order.Adapt<OrderDetailVM>();
                try
                {
                    List<CartVM> carts = new List<CartVM>();
                    if (Order.Status == (int)OrderStatus.done)
                    {
                        foreach (var cart in Order.OrderDetails)
                        {
                            carts.Add(cart.Adapt<CartVM>());
                        }
                    }
                    else
                    {
                        foreach (var cart in Order.Carts)
                        {
                            carts.Add(cart.Adapt<CartVM>());
                        }
                    }
                    result.CartVMs = carts;
                    result.Device_Id = _userManager.FindByIdAsync(Order.UserId).Result.Device_Id;
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Message = ex.Message });
                }
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Confirm/{id}")]
        public ActionResult ConfirmOrder(Guid id)
        {
            try
            {
                var order = _OrderService.GetOrder(id);
                if (order == null) return NotFound(new { Message = "Khong tim thay Order" });
                order.Status = (int)OrderStatus.confirmed;
                foreach (var cart in order.Carts)
                {
                    cart.Status = (int)CartStatus.ordered;
                }
                _OrderService.EditOrder(order, _userManager.GetUserAsync(User).Result.FullName);
                _OrderService.SaveOrder();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Done/{id}")]
        public ActionResult DoneOrder(Guid id)
        {
            try
            {
                var order = _OrderService.GetOrder(id);
                if (order == null) return NotFound(new { Message = "Khong tim thay Order" });
                if (order.Status != (int)OrderStatus.confirmed)
                {
                    return BadRequest(new { Message = "Order Status != Confimed" });
                }

                var username = _userManager.GetUserAsync(User).Result.FullName;
                //Update Order
                order.Status = (int)OrderStatus.done;

                //Delete Cart and move to OrderDetail
                var carts = order.Carts;
                var warehouses = carts.Select(_ => _.WareHouse).ToList();
                var orderDetails = carts.Adapt<List<OrderDetailsVM>>().Adapt<List<OrderDetail>>();
                foreach (var cart in carts)
                {
                    _cartService.RemoveCart(cart);
                }
                order.OrderDetails = orderDetails;
                _OrderService.EditOrder(order, username);
                _OrderService.SaveOrder();

                //Update Warehouse
                foreach (var warehouse in warehouses)
                {
                    var quantity = orderDetails.Where(_ => _.ProductId == warehouse.ProductId).Select(_ => _.Quantity).FirstOrDefault();
                    if (quantity == 0)
                    {
                        return BadRequest(new { Message = warehouse.Id + " not exist or ProductId sai" });
                    }
                    warehouse.Purchased = warehouse.Purchased + quantity;
                    warehouse.Ordered = warehouse.Ordered + quantity;
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
