using MRS.Data.Infrastructure;
using MRS.Data.Repositories;
using MRS.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MRS.Service
{
    public interface INewsService
    {
        IEnumerable<News> GetNewss();
        IEnumerable<News> GetNewss(Expression<Func<News, bool>> where);
        News GetNews(Guid id);
        void CreateNews(News News, string username);
        void EditNews(News News, string username);
        void RemoveNews(News News);
        void SaveNews();
    }

    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public NewsService(INewsRepository NewsRepository, IUnitOfWork unitOfWork)
        {
            this._newsRepository = NewsRepository;
            this._unitOfWork = unitOfWork;
        }

        public void CreateNews(News News, string username)
        {
            News.DateCreated = DateTime.Now;
            News.UserCreated = username;
            _newsRepository.Add(News);
        }

        public void EditNews(News News, string username)
        {
            News.DateUpdated = DateTime.Now;
            News.UserUpdated = username;
            _newsRepository.Update(News);
        }

        public News GetNews(Guid id)
        {
            return _newsRepository.GetById(id);
        }

        public IEnumerable<News> GetNewss()
        {
            return _newsRepository.GetAll();
        }

        public void SaveNews()
        {
            _unitOfWork.Commit();
        }

        public void RemoveNews(News News)
        {
            _newsRepository.Delete(News);
        }

        public IEnumerable<News> GetNewss(Expression<Func<News, bool>> where)
        {
            return _newsRepository.GetMany(where);
        }
    }
}
