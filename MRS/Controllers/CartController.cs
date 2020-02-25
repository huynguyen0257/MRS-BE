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
using MRS.ViewModels;

namespace MRS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _CartService;
        private readonly UserManager<User> _userManager;
        private readonly IWareHouseService _wareHouseService;
        private readonly IOrderService _orderService;

        public CartController(ICartService cartService, UserManager<User> userManager, IWareHouseService wareHouseService, IOrderService orderService)
        {
            _CartService = cartService;
            _userManager = userManager;
            _wareHouseService = wareHouseService;
            _orderService = orderService;
        }

        [HttpGet]
        public ActionResult GetAll()
        {
            try
            {
                return Ok(_CartService.GetCarts().Select(c => c.Adapt<CartVM>()));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Create([FromBody] CartCM model)
        {
            try
            {
                var Cart = model.Adapt<Cart>();
                Cart.DateCreated = DateTime.Now;
                var warehouse = _wareHouseService.GetWareHouses(w => w.ProductId == model.ProductId).FirstOrDefault();
                if (warehouse == null) return NotFound(new { Message = "Khong tim thay Product" });
                Cart.WareHouse = warehouse;
                var user = _userManager.GetUserAsync(User).Result;
                if (user != null)
                {
                    _CartService.CreateCart(Cart, user.FullName);
                }
                else
                {
                    _CartService.CreateCart(Cart, null);
                }
                _CartService.SaveCart();
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("LIST")]
        public ActionResult Create(List<CartCM> models)
        {
            List<Guid> ids = new List<Guid>();
            try
            {
                var carts = models.Adapt<List<Cart>>();
                foreach (var cart in carts)
                {
                    cart.DateCreated = DateTime.Now;
                    var warehouse = _wareHouseService.GetWareHouses(w => w.ProductId == cart.ProductId).FirstOrDefault();
                    if (warehouse == null) return NotFound(new { Message = "Khong tim thay Product" });
                    cart.WareHouse = warehouse;
                    var user = _userManager.GetUserAsync(User).Result;
                    _CartService.CreateCart(cart, user.FullName);
                    ids.Add(cart.Id);
                }
                _CartService.SaveCart();
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                string idCartRemoceEx = "";
                foreach (var id in ids)
                {
                    try
                    {
                        _CartService.RemoveCart(id);
                    }
                    catch (Exception e)
                    {
                        idCartRemoceEx += id + "\t,";
                    }
                }
                if (String.IsNullOrEmpty(idCartRemoceEx))
                {
                    return BadRequest(new { Message = ex.Message });
                }
                return BadRequest(new { Message = "Create fail ==> Remove fail in : " + idCartRemoceEx });
            }
        }

        [HttpPut]
        public ActionResult Update(CartUM model)
        {
            try
            {
                var Cart = _CartService.GetCart(model.Id);
                var oldQuantity = Cart.Quantity;
                var user = _userManager.GetUserAsync(User).Result;
                if (Cart != null)
                {
                    Cart = model.Adapt(Cart);
                    if (user != null)
                    {
                        _CartService.EditCart(Cart, user.FullName, oldQuantity);
                    }
                    else
                    {
                        _CartService.EditCart(Cart, null, oldQuantity);
                    }
                    _CartService.SaveCart();
                    return Ok();
                }
                else
                {
                    return BadRequest(new { Message = "CartId sai" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                var Cart = _CartService.GetCart(id);
                if (Cart != null)
                {
                    _CartService.RemoveCart(Cart);
                    _CartService.SaveCart();
                    return Ok();
                }
                else
                {
                    return BadRequest(new { Message = "CartId sai" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        #region Order
        [HttpPost("Order")]
        public ActionResult Order([FromBody]OrderCM model)
        {
            var cart = _CartService.GetCarts(_ => model.CartIds.Contains(_.Id)).ToList();
            if (cart.Count() != model.CartIds.Count)
            {
                string idError = "";
                foreach (var cartId in model.CartIds)
                {
                    if (cart.Where(_ => _.Id == cartId).FirstOrDefault() == null) idError += cartId + "\t,";
                }
                return BadRequest(new { Message = "CartId not exist : " + idError });
            }
            var order = model.Adapt<Order>();
            order.Carts = cart;
            _orderService.CreateOrder(order, _userManager.GetUserAsync(User).Result.FullName);

            return Ok();
        }
        #endregion

    }
}