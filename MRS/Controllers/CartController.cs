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
    [Authorize(Roles = "Customer")]
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
                return Ok(_CartService.GetCarts(_ => _.UserId == _userManager.GetUserAsync(User).Result.Id && _.Status == (int)CartStatus.waiting && _.OrderId == null).Select(c => c.Adapt<CartVM>()));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        private bool IsExistedCart(Cart cart, Guid productId, string userId)
        {
            var result = cart.ProductId == productId;
            result = result && cart.UserId.Contains(userId);
            result = result && cart.Status == (int)CartStatus.waiting;
            result = result && cart.OrderId == null;
            return result;
        }

        [HttpPost]
        public ActionResult Create([FromBody] CartCM model)
        {
            try
            {
                var user = _userManager.GetUserAsync(User).Result;
                var currentCart = _CartService.GetCarts(_ => IsExistedCart(_,model.ProductId,user.Id)).FirstOrDefault();
                if (currentCart != null)
                {
                    //return BadRequest(new { Message = "User nay da add Product roi ==> Update Quantity di!" });
                    currentCart.Quantity += model.Quantity;
                    _CartService.EditCart(currentCart, user.FullName);
                    _CartService.SaveCart();
                    return Ok(new { Message = String.Format("Updated quantity for Fullname : {0}, ProductName: {1} ", user.FullName, currentCart.ProductName) });
                }

                var Cart = model.Adapt<Cart>();
                Cart.DateCreated = DateTime.Now;
                var warehouse = _wareHouseService.GetWareHouses(w => w.ProductId == model.ProductId).FirstOrDefault();
                if (warehouse == null) return NotFound(new { Message = "Khong tim thay Product" });
                Cart.WareHouse = warehouse;
                _CartService.CreateCart(Cart, user.FullName, user.Id);
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
        public ActionResult Create([FromBody]List<CartCM> models)
        {
            var user = _userManager.GetUserAsync(User).Result;
            List<Guid> ids = new List<Guid>();
            try
            {
                var cartCheckeds = new List<CartCM>();
                foreach (var model in models)
                {
                    if (cartCheckeds.Where(_ => _.ProductId == model.ProductId).FirstOrDefault() != null) cartCheckeds.Where(_ => _.ProductId == model.ProductId).FirstOrDefault().Quantity += model.Quantity;
                    else cartCheckeds.Add(model);
                    if (_CartService.GetCarts(_ => _.UserId == user.Id).FirstOrDefault() != null) return BadRequest(new { Message = "User have cart on System ... Take POST api/cart!" });
                }
                var carts = cartCheckeds.Adapt<List<Cart>>();
                foreach (var cart in carts)
                {
                    cart.DateCreated = DateTime.Now;
                    var warehouse = _wareHouseService.GetWareHouses(w => w.ProductId == cart.ProductId).FirstOrDefault();
                    if (warehouse == null) return NotFound(new { Message = "Khong tim thay Product" });
                    cart.WareHouse = warehouse;
                    _CartService.CreateCart(cart, user.FullName, user.Id);
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
                var user = _userManager.GetUserAsync(User).Result;
                if (Cart != null)
                {
                    Cart = model.Adapt(Cart);
                    _CartService.EditCart(Cart, user.FullName);
                    _CartService.SaveCart();
                    var cartRemove = _CartService.GetCarts(_ => _.Quantity == 0).FirstOrDefault();
                    if (cartRemove != null)
                    {
                        _CartService.RemoveCart(cartRemove);
                        _CartService.SaveCart();
                    }
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
            try
            {
                #region Check and Get cart
                var cart = _CartService.GetCarts(_ => model.CartIds.Contains(_.Id)).ToList();
                if (cart.Count() != model.CartIds.Count)
                {
                    string idError = "";
                    foreach (var cartId in model.CartIds)
                    {
                        if (cart.Where(_ => _.Id == cartId).FirstOrDefault() == null) idError += cartId + "\t,";
                    }
                    if (!String.IsNullOrEmpty(idError))
                    {
                        return BadRequest(new { Message = "CartId not exist : " + idError });
                    }
                }
                #endregion
                if (cart.Where(_ => _.OrderId != null).FirstOrDefault() != null) return BadRequest(new { Message = "Have 1 or more carts have been ordered" });
                var order = model.Adapt<Order>();
                order.UserId = cart.FirstOrDefault().UserId;
                order.Carts = cart;
                _orderService.CreateOrder(order, _userManager.GetUserAsync(User).Result.FullName);
                _orderService.SaveOrder();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            
        }
        #endregion

    }
}