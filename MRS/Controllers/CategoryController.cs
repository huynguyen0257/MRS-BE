﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MRS.Model;
using MRS.Service;
using MRS.Utils;
using MRS.ViewModels;

namespace MRS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetAll()
        {
            try
            {
                //var test = new List<Category> { new Category() { Name = "Test Cat", Id = Guid.NewGuid() } };
                return Ok(_categoryService.GetCategorys(c => !c.IsDelete).Select(c => c.Adapt<CategoryVM>()));
                //return Ok(test);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public ActionResult Get(Guid id)
        {
            var category = _categoryService.GetCategory(id);
            if (category != null && !category.IsDelete)
            {
                try
                {
                    return Ok(category.Adapt<CategoryVM>());
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

        [AllowAnonymous]
        [HttpGet("{id}/Products")]
        public ActionResult GetProductByCateId(Guid id,int index = 1, int pageSize = 10)
        {
            var result = new List<ProductVM>();
            var category = _categoryService.GetCategory(id);
            if (category != null && !category.IsDelete)
            {
                try
                {
                    return Ok(category.Products.ToPageList<ProductVM, Product>(index,pageSize));
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
        public ActionResult Create(CategoryCM model)
        {
            try
            {
                var category = model.Adapt<Category>();
                category.DateCreated = DateTime.Now;
                _categoryService.CreateCategory(category, null);
                _categoryService.SaveCategory();
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public ActionResult Update(CategoryUM model)
        {
            try
            {
                var category = _categoryService.GetCategory(model.Id);
                if (category != null)
                {
                    category = model.Adapt(category);
                    _categoryService.EditCategory(category, null);
                    _categoryService.SaveCategory();
                    return Ok();
                }
                else
                {
                    return BadRequest("CategoryId sai");
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
                var category = _categoryService.GetCategory(id);
                if (category != null)
                {
                    _categoryService.RemoveCategory(category, null);
                    _categoryService.SaveCategory();
                    return Ok();
                }
                else
                {
                    return BadRequest("CategoryId sai");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}