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
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IWareHouseService _wareHouseService;
        private readonly UserManager<User> _userManager;

        public OrderController(IOrderService orderService, ICartService cartService, IOrderDetailService orderDetailService, IWareHouseService wareHouseService, UserManager<User> userManager)
        {
            _orderService = orderService;
            _cartService = cartService;
            _orderDetailService = orderDetailService;
            _wareHouseService = wareHouseService;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet("MySelf")]
        public ActionResult GetMySelf(int index = 1, int pageSize = 5)
        {
            try
            {
                var user = _userManager.GetUserAsync(User).Result;
                var orders = _orderService.GetOrders(_ => _.UserId  .Contains(user.Id));
                return Ok(orders.ToPageList<OrderVM, Order>(index, pageSize));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("All")]
        public ActionResult GetAll(int? type, int index = 1, int pageSize = 5)
        {
            try
            {
                if (type == null)
                {
                    return Ok(_orderService.GetOrders().ToPageList<OrderVM,Order>(index,pageSize));
                }
                else 
                {
                    return Ok(_orderService.GetOrders(_ => _.Status == type).ToPageList<OrderVM, Order>(index, pageSize));
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
            var Order = _orderService.GetOrder(id);
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
                var order = _orderService.GetOrder(id);
                if (order == null) return NotFound(new { Message = "Khong tim thay Order" });
                order.Status = (int)OrderStatus.confirmed;
                foreach (var cart in order.Carts)
                {
                    cart.Status = (int)CartStatus.ordered;
                }
                _orderService.EditOrder(order, _userManager.GetUserAsync(User).Result.FullName);
                _orderService.SaveOrder();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Refuse/{id}")]
        public ActionResult RefuseOrder(Guid id)
        {
            try
            {
                var order = _orderService.GetOrder(id);
                if (order == null) return NotFound(new { Message = "Khong tim thay Order" });
                order.Status = (int)OrderStatus.refuse;
                foreach (var cart in order.Carts)
                {
                    cart.Status = (int)CartStatus.refused;
                }
                _orderService.EditOrder(order, _userManager.GetUserAsync(User).Result.FullName);
                _orderService.SaveOrder();

                //Update Warehouse
                var carts = order.Carts;
                var warehouses = carts.Select(_ => _.WareHouse).ToList();
                foreach (var warehouse in warehouses)
                {
                    var quantity = carts.Where(_ => _.ProductId == warehouse.ProductId).Select(_ => _.Quantity).FirstOrDefault();
                    if (quantity == 0)
                    {
                        return BadRequest(new { Message = warehouse.Id + " not exist or ProductId sai" });
                    }
                    warehouse.Avaiable = warehouse.Avaiable + quantity;
                    warehouse.Ordered = warehouse.Ordered - quantity;
                    _wareHouseService.EditWareHouse(warehouse);
                }
                _wareHouseService.SaveWareHouse();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        private int GetScore(float totalPrice)
        {
            if (totalPrice > 0)
            {
                return (int)totalPrice / 10000;
            }
            else
            {
                throw new Exception("TotalPrice = 0");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Done/{id}")]
        public async Task<ActionResult> DoneOrderAsync(Guid id)
        {
            try
            {
                var order = _orderService.GetOrder(id);
                if (order == null) return NotFound(new { Message = "Khong tim thay Order" });
                if (order.Status != (int)OrderStatus.confirmed)
                {
                    return BadRequest(new { Message = "Order Status != Confimed" });
                }

                //Update User's Score and level
                var user = _userManager.GetUserAsync(User).Result;
                user.Score += GetScore(order.Price);
                if (user.Level == (int)UserLevel.Member)
                {
                    user.Level = user.Score >= 200 ? (int)UserLevel.Gold : (int)UserLevel.Member;
                }
                else if (user.Level == (int)UserLevel.Gold)
                {
                    user.Level = user.Score >= 400 ? (int)UserLevel.Platinum : (int)UserLevel.Gold;
                }
                var update = await _userManager.UpdateAsync(user);
                if (!update.Succeeded)
                {
                    return BadRequest(new { Message = "Update user fail!" });
                }

                //Update Order
                order.Status = (int)OrderStatus.done;

                //Delete Cart and move to OrderDetail
                var carts = order.Carts;
                var warehouses = carts.Select(_ => _.WareHouse).ToList();
                var orderDetails = carts.Adapt<List<OrderDetailsVM>>().Adapt<List<OrderDetail>>();
                foreach (var orderDetail in orderDetails)
                {
                    orderDetail.Status = (int)CartStatus.done;
                }
                foreach (var cart in carts)
                {
                    _cartService.RemoveCart(cart);
                }
                order.OrderDetails = orderDetails;
                _orderService.EditOrder(order, user.FullName);
                _orderService.SaveOrder();

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
                    _wareHouseService.EditWareHouse(warehouse);
                }
                _wareHouseService.SaveWareHouse();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
