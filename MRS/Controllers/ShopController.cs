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
    public class ShopController : ControllerBase
    {
        private readonly IShopService _shopService;

        public ShopController(IShopService ShopService)
        {
            _shopService = ShopService;
        }

        [HttpGet]
        public ActionResult GetAll()
        {
            try
            {
                return Ok(_shopService.GetShops(c => !c.IsDelete).Select(c => c.Adapt<ShopVM>()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult Get(Guid id)
        {
            var Shop = _shopService.GetShop(id);
            if (Shop != null && !Shop.IsDelete)
            {
                try
                {
                    return Ok(Shop.Adapt<ShopDetailVM>());
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public ActionResult Create(ShopCM model)
        {
            try
            {
                var shop = model.Adapt<Shop>();
                shop.DateCreated = DateTime.Now;
                _shopService.CreateShop(shop, null);
                _shopService.SaveShop();
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public ActionResult Update(ShopUM model)
        {
            try
            {
                var Shop = _shopService.GetShop(model.Id);
                if (Shop != null)
                {
                    Shop = model.Adapt(Shop);
                    _shopService.EditShop(Shop, null);
                    _shopService.SaveShop();
                    return Ok();
                }
                else
                {
                    return BadRequest("ShopId sai");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                var Shop = _shopService.GetShop(id);
                if (Shop != null)
                {
                    _shopService.RemoveShop(Shop, null);
                    _shopService.SaveShop();
                    return Ok();
                }
                else
                {
                    return BadRequest("ShopId sai");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}