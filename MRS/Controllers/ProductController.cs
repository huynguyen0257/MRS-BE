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
using Newtonsoft.Json;

namespace MRS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProductController : ControllerBase
    {
        public class Error
        {
            public string Message { get; set; }
        }
        private readonly IProductService _productService;
        private readonly UserManager<User> _userManager;
        private readonly IFileService _fileService;

        public ProductController(IProductService productService, UserManager<User> userManager, IFileService fileService)
        {
            _productService = productService;
            _userManager = userManager;
            _fileService = fileService;
        }

        [HttpGet]
        public ActionResult Get(string name, int index = 1, int pageSize = 5)
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
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("Images")]
        public async Task<ActionResult> GetImages(string fileName)
        {
            try
            {
                var result = new List<FileStreamResult>();
                var file = await _fileService.GetFileAsync(fileName);
                return File(file.Stream, file.ContentType);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Create([FromBody]ProductCM model)
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
                _productService.CreateProduct(product, _userManager.GetUserAsync(User).Result.FullName);
                _productService.SaveProduct();
                return StatusCode(201, product.Id);
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
                    _productService.EditProduct(product, null);
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

        [HttpPut("Images")]
        public ActionResult UpdateloadImages([FromForm]List<IFormFile> images, Guid id, bool isMain = true)
        {
            try
            {
                var product = _productService.GetProduct(id);
                List<string> imageNames = new List<string>();
                //Images
                if (isMain)
                {
                    var oldMainImage = product.MainImage;
                    var image = images.FirstOrDefault();
                    var filename = _fileService.SaveFile(FilePath.product, image);
                    product.MainImage = filename.Result;
                    if (oldMainImage != null)
                    {
                        _fileService.DeleteFile(oldMainImage);
                    }
                }
                else
                {
                    foreach (var image in images)
                    {
                        var filename = _fileService.SaveFile(FilePath.product, image);
                        imageNames.Add(filename.Result);
                    }
                    var oldImageNames = JsonConvert.DeserializeObject<List<string>>(product.Images);
                    foreach (var oldImageName in oldImageNames)
                    {
                        _fileService.DeleteFile(oldImageName);
                    }
                    product.Images = JsonConvert.SerializeObject(imageNames);
                }
                _productService.EditProduct(product, _userManager.GetUserAsync(User).Result.FullName);
                _productService.SaveProduct();
                return Ok();
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
                var product = _productService.GetProduct(id);
                if (product != null)
                {
                    var mainImages = product.MainImage;
                    var images = JsonConvert.DeserializeObject<List<string>>(product.Images);
                    _productService.RemoveProduct(product, _userManager.GetUserAsync(User).Result.FullName);
                    _productService.SaveProduct();
                    _fileService.DeleteFile(mainImages);
                    foreach (var image in images)
                    {
                        _fileService.DeleteFile(image);
                    }
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