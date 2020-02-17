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
    public class ProductController : ControllerBase
    {
        public IProductService _productService { get; set; }

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                var products = _productService.GetProducts(p => !p.IsDelete).Select(p => p.Adapt<ProductVM>());
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet("{id}")]
        public ActionResult GetDetail(Guid id)
        {
            var product = _productService.GetProduct(id);
            try
            {
                if (product != null && !product.IsDelete)
                {
                    return Ok(product.Adapt<ProductDetailVM>());
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Create(ProductCM model)
        {
            try
            {
                var product = model.Adapt<Product>();
                product.DateCreated = DateTime.Now;
                _productService.CreateProduct(product,null);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public ActionResult Update(ProductUM model)
        {
            try
            {
                var product = _productService.GetProduct(model.Id);
                if (product != null)
                {
                    product = model.Adapt(product);
                    _productService.EditProduct(product,null);
                    return Ok();
                }
                else
                {
                    return BadRequest("ProductId sai");
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
                var product = _productService.GetProduct(id);
                if (product != null)
                {
                    _productService.RemoveProduct(product,null);
                    return Ok();
                }
                else
                {
                    return BadRequest("ProductId sai");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}