using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MRS.Model;
using MRS.Service;
using MRS.Utils;
using MRS.ViewModels;

namespace MRS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        public class Error
        {
            public string Message { get; set; }
        }
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public ActionResult Get(string name,int index = 1 , int pageSize = 5)
        {
            try
            {
                name = name != null ? name : "";
                var products = _productService.GetProducts(p => !p.IsDelete && p.Name.Contains(name)).OrderByDescending(_ => _.DateCreated);
                return Ok(products.ToPageList<ProductVM, Product>(index, pageSize));
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
                product.WareHouse = new WareHouse
                {
                    Quantity = model.Quantity,
                    Avaiable = model.Quantity
                };
                _productService.CreateProduct(product,null);
                _productService.SaveProduct();
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
            var error = new Error();
            try
            {
                var product = _productService.GetProduct(model.Id);
                if (product != null)
                {
                    product = model.Adapt(product);
                    if (product.WareHouse != null)
                    {
                        var warehouse = product.WareHouse;
                        var avaiable = model.Quantity - warehouse.Ordered - warehouse.Purchased;
                        if (avaiable < 0)
                        {
                            //error.Message += "Khong duoc giam Quantity";
                            //return BadRequest(error);
                            throw new Exception("Khong update duoc vi so luong hang trong kho kha dung la: " + warehouse.Avaiable);
                        }
                        warehouse.Quantity = model.Quantity;
                        warehouse.Avaiable = avaiable;
                    }
                    _productService.EditProduct(product,null);
                    _productService.SaveProduct();
                }
                else
                {
                    error.Message += "ProductId sai";
                }
            }
            catch (Exception ex)
            {
                error.Message += ex.Message;
            }
            if (String.IsNullOrEmpty(error.Message))
            {
                return Ok();
            }
            else
            {
                return BadRequest(error);
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
                    _productService.SaveProduct();
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