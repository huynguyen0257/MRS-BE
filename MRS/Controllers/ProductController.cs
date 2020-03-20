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

    [Authorize(Roles = "Admin, Customer")]
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

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpGet("{id}")]
        public ActionResult GetDetail(Guid id)
        {
            var product = _productService.GetProduct(id);
            try
            {
                if (product != null && !product.IsDelete)
                {
                    var result = product.Adapt<ProductDetailVM>();
                    var user = _userManager.GetUserAsync(User).Result;
                    if (user != null)
                    {
                        result.IsLiked = product.PopularProducts.Where(_ => _.UserId == user.Id).FirstOrDefault() != null;
                    }
                    return Ok(result);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/CustomerLiked")]
        public ActionResult GetCustomerLiked(Guid id)
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

        [Authorize(Roles = "Customer")]
        [HttpGet("LikeByMyself")]
        public ActionResult GetLikeByMyself(int index = 1, int pageSize = 5)
        {
            var user = _userManager.GetUserAsync(User).Result;
            var products = _productService.GetProducts(_ => _.PopularProducts.Where(p => p.UserId == user.Id).FirstOrDefault() != null);
            try
            {
                return Ok(products.ToPageList<ProductVM,Product>(index, pageSize));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [AllowAnonymous]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Customer")]
        [HttpPut("{id}/Like")]
        public ActionResult Update(Guid id)
        {
            try
            {
                var product = _productService.GetProduct(id);
                if (product == null && !product.IsDelete) return BadRequest(new { Message = "ProductId sai" });
                if (product.PopularProducts == null)
                {
                    product.PopularProducts = new List<PopularProduct>();
                }
                var userId = _userManager.GetUserId(User);
                var popularProduct = product.PopularProducts.Where(_ => _.UserId == userId && _.ProductId == product.Id).FirstOrDefault();
                if (popularProduct != null)
                {
                    product.PopularProducts.Remove(popularProduct);
                    product.NumberOfLike--;
                }
                else
                {
                    product.PopularProducts.Add(new PopularProduct
                    {
                        Product = product,
                        UserId = _userManager.GetUserId(User)
                    });
                    product.NumberOfLike++;
                }
                _productService.EditProduct(product, null);
                _productService.SaveProduct();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
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
                    if (product.Images != null)
                    {
                        var oldImageNames = JsonConvert.DeserializeObject<List<string>>(product.Images);
                        foreach (var oldImageName in oldImageNames)
                        {
                            _fileService.DeleteFile(oldImageName);
                        }
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

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                var product = _productService.GetProduct(id);
                if (product != null)
                {
                    var mainImages = product.MainImage;
                    if (mainImages != null)
                    {
                        _fileService.DeleteFile(mainImages);
                    }
                    if (product.Images != null)
                    {
                        var images = JsonConvert.DeserializeObject<List<string>>(product.Images);
                        foreach (var image in images)
                        {
                            _fileService.DeleteFile(image);
                        }
                    }
                    _productService.RemoveProduct(product, _userManager.GetUserAsync(User).Result.FullName);
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