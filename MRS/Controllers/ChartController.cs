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
    public class ChartController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IWareHouseService _wareHouseService;

        public ChartController(IOrderService orderService, IWareHouseService wareHouseService)
        {
            _orderService = orderService;
            _wareHouseService = wareHouseService;
        }

        [HttpGet("SoldProduct")]
        public ActionResult GetSoldProduct()
        {
            //var orders = _orderService.GetOrders(o => o.Status == (int)OrderStatus.done);
            //var orderDetails = new List<OrderDetail>();
            //var result = new List<SoldProductVM>();
            //foreach (var order in orders)
            //{
            //    foreach (var detail in order.OrderDetails)
            //    {
            //        var b = result.Where(c => c.ProductId == detail.ProductId).FirstOrDefault();
            //        if (b != null) //existed
            //        {
            //            b.Quantity += detail.Quantity;
            //        }
            //        else //not existed
            //        {
            //            result.Add(new SoldProductVM
            //            {
            //                ProductId = detail.ProductId,
            //                ProductName = detail.ProductName,
            //                Quantity = detail.Quantity
            //            });
            //        }
            //    }
            //}
            var result = new List<InventoryVM>();
            var wareHouses = _wareHouseService.GetWareHouses();
            foreach (var wareHouse in wareHouses)
            {
                result.Add(new InventoryVM
                {
                    ProductId = wareHouse.ProductId,
                    ProductName = wareHouse.Product.Name,
                    Quantity = wareHouse.Purchased
                });
            }
            return Ok(result);
        }

        [HttpGet("Inventory")]
        public ActionResult GetInventory()
        {
            var result = new List<InventoryVM>();
            var wareHouses = _wareHouseService.GetWareHouses();
            foreach (var wareHouse in wareHouses)
            {
                result.Add(new InventoryVM
                {
                    ProductId = wareHouse.ProductId,
                    ProductName = wareHouse.Product.Name,
                    Quantity = wareHouse.Avaiable
                });
            }
            return Ok(result);
        }

        [HttpGet("SalePrice")]
        public ActionResult GetInventoryPrice()
        {
            var wareHouses = _wareHouseService.GetWareHouses();
            float inventoryPrice = 0;
            float salePrice = 0;
            foreach (var wareHouse in wareHouses)
            {
                inventoryPrice += wareHouse.Avaiable * wareHouse.Product.Price;
                salePrice += wareHouse.Purchased * wareHouse.Product.Price;
            }
            var result = new SalePriceVM
            {
                InventoryPrice = inventoryPrice,
                SalePrice = salePrice,
                TotalPrice = inventoryPrice + salePrice
            };
            return Ok(result);
        }
    }
}