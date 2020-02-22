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
    public class WareHouseController : ControllerBase
    {
        private readonly IWareHouseService _WareHouseService;

        public WareHouseController(IWareHouseService WareHouseService)
        {
            _WareHouseService = WareHouseService;
        }

        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                try
                {
                    var WareHouses = _WareHouseService.GetWareHouses().Select(w => w.Adapt<WareHouseVM>());
                    return Ok(WareHouses);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}