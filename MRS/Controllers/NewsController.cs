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
    public class NewsController : ControllerBase
    {
        private readonly INewsService _NewsService;

        public NewsController(INewsService NewsService)
        {
            _NewsService = NewsService;
        }

        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                var Newss = _NewsService.GetNewss(n => !n.IsHided).Select(p => p.Adapt<NewsVM>());
                return Ok(Newss);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("Hided")]
        public ActionResult GetHided()
        {
            try
            {
                var Newss = _NewsService.GetNewss(n => n.IsHided).Select(p => p.Adapt<NewsVM>());
                return Ok(Newss);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("{id}")]
        public ActionResult GetDetail(Guid id)
        {
            var News = _NewsService.GetNews(id);
            try
            {
                if (News != null && !News.IsHided)
                {
                    return Ok(News.Adapt<NewsDetailVM>());
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Create(NewsCM model)
        {
            try
            {
                var News = model.Adapt<News>();
                News.DateCreated = DateTime.Now;
                _NewsService.CreateNews(News, null);
                _NewsService.SaveNews();
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public ActionResult Update(NewsUM model)
        {
            try
            {
                var News = _NewsService.GetNews(model.Id);
                if (News != null)
                {
                    News = model.Adapt(News);
                    _NewsService.EditNews(News, null);
                    _NewsService.SaveNews();
                    return Ok();
                }
                else
                {
                    return BadRequest("NewsId sai");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Hide")]
        public ActionResult HideNews(List<Guid> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var news = _NewsService.GetNews(id);
                    if (news != null)
                    {
                        news.IsHided = true;
                        _NewsService.EditNews(news, null);
                    }
                    else
                    {
                        return BadRequest("NewsId sai");
                    }
                }
                _NewsService.SaveNews();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UnHide")]
        public ActionResult UnHideNews(List<Guid> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var news = _NewsService.GetNews(id);
                    if (news != null)
                    {
                        news.IsHided = false;
                        _NewsService.EditNews(news, null);
                    }
                    else
                    {
                        return BadRequest("NewsId sai");
                    }
                }
                _NewsService.SaveNews();
                return Ok();
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
                var News = _NewsService.GetNews(id);
                if (News != null)
                {
                    _NewsService.RemoveNews(News);
                    _NewsService.SaveNews();
                    return Ok();
                }
                else
                {
                    return BadRequest("NewsId sai");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}