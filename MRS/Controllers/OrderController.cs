using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MRS.Model;
using MRS.Service;
using MRS.ViewModels;

namespace MRS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _OrderService;

        public OrderController(IOrderService OrderService)
        {
            _OrderService = OrderService;
        }

        [HttpGet]
        public ActionResult GetAll()
        {
            try
            {
                return Ok(_OrderService.GetOrders().Select(c => c.Adapt<OrderVM>()));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public ActionResult Get(Guid id)
        {
            var Order = _OrderService.GetOrder(id);
            if (Order != null)
            {
                var result = Order.Adapt<OrderDetailVM>();
                try
                {
                    foreach (var cart in Order.Carts)
                    {
                        result.CartVMs.Add(cart.Adapt<CartVM>());
                    }
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

        //TODO : lam "confirmed" order theo orderId , check "done" order theo customername vs list cart
    }
}