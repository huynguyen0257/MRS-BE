using MRS.Data.Infrastructure;
using MRS.Data.Repositories;
using MRS.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MRS.Service
{
    public interface ICategoryService
    {
        IEnumerable<Category> GetCategorys();
        IEnumerable<Category> GetCategorys(Expression<Func<Category, bool>> where);
        Category GetCategory(Guid id);
        void CreateCategory(Category Category);
        void EditCategory(Category Category);
        void RemoveCategory(Category Category);
        void SaveCategory();
    }

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _CategoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(ICategoryRepository CategoryRepository, IUnitOfWork unitOfWork)
        {
            this._CategoryRepository = CategoryRepository;
            this._unitOfWork = unitOfWork;
        }

        public void CreateCategory(Category Category)
        {
            _CategoryRepository.Add(Category);
        }

        public void EditCategory(Category Category)
        {
            var entity = _CategoryRepository.GetById(Category.Id);
            entity = Category;
            _CategoryRepository.Update(entity);
        }

        public Category GetCategory(Guid id)
        {
            return _CategoryRepository.GetById(id);
        }

        public IEnumerable<Category> GetCategorys()
        {
            return _CategoryRepository.GetAll();
        }

        public void SaveCategory()
        {
            _unitOfWork.Commit();
        }

        public void RemoveCategory(Category Category)
        {
            Category.IsDelete = true;
            EditCategory(Category);
        }

        public IEnumerable<Category> GetCategorys(Expression<Func<Category, bool>> where)
        {
            return _CategoryRepository.GetMany(where);
        }
    }
}
