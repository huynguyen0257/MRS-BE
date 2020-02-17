using MRS.Data.Infrastructure;
using MRS.Data.Repositories;
using MRS.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MRS.Service
{
    public interface IProductService
    {
        IEnumerable<Product> GetProducts();
        IEnumerable<Product> GetProducts(Expression<Func<Product, bool>> where);
        Product GetProduct(Guid id);
        void CreateProduct(Product Product, string username);
        void EditProduct(Product Product, string username);
        void RemoveProduct(Product Product, string username);
        void SaveProduct();
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepository _ProductRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IProductRepository ProductRepository, IUnitOfWork unitOfWork)
        {
            this._ProductRepository = ProductRepository;
            this._unitOfWork = unitOfWork;
        }

        public void CreateProduct(Product Product, string username)
        {
            Product.DateCreated = DateTime.Now;
            Product.UserCreated = username;
            _ProductRepository.Add(Product);
        }

        public void EditProduct(Product Product, string username)
        {
            Product.DateUpdated = DateTime.Now;
            Product.UserUpdated = username;
            _ProductRepository.Update(Product);
        }

        public Product GetProduct(Guid id)
        {
            return _ProductRepository.GetById(id);
        }

        public IEnumerable<Product> GetProducts()
        {
            return _ProductRepository.GetAll();
        }

        public void SaveProduct()
        {
            _unitOfWork.Commit();
        }

        public void RemoveProduct(Product Product,string username)
        {
            Product.IsDelete = true;
            EditProduct(Product,username);
        }

        public IEnumerable<Product> GetProducts(Expression<Func<Product, bool>> where)
        {
            return _ProductRepository.GetMany(where);
        }
    }
}
