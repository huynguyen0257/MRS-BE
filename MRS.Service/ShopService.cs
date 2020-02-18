using MRS.Data.Infrastructure;
using MRS.Data.Repositories;
using MRS.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MRS.Service
{
    public interface IShopService
    {
        IEnumerable<Shop> GetShops();
        IEnumerable<Shop> GetShops(Expression<Func<Shop, bool>> where);
        Shop GetShop(Guid id);
        void CreateShop(Shop Shop, string username);
        void EditShop(Shop Shop, string username);
        void RemoveShop(Shop Shop, string username);
        void SaveShop();
    }

    public class ShopService : IShopService
    {
        private readonly IShopRepository _ShopRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ShopService(IShopRepository ShopRepository, IUnitOfWork unitOfWork)
        {
            this._ShopRepository = ShopRepository;
            this._unitOfWork = unitOfWork;
        }

        public void CreateShop(Shop Shop, string username)
        {
            Shop.DateCreated = DateTime.Now;
            Shop.UserCreated = username;
            _ShopRepository.Add(Shop);
        }

        public void EditShop(Shop Shop, string username)
        {
            Shop.DateUpdated = DateTime.Now;
            Shop.UserUpdated = username;
            _ShopRepository.Update(Shop);
        }

        public Shop GetShop(Guid id)
        {
            return _ShopRepository.GetById(id);
        }

        public IEnumerable<Shop> GetShops()
        {
            return _ShopRepository.GetAll();
        }

        public void SaveShop()
        {
            _unitOfWork.Commit();
        }

        public void RemoveShop(Shop Shop, string username)
        {
            Shop.IsDelete = true;
            EditShop(Shop, username);
        }

        public IEnumerable<Shop> GetShops(Expression<Func<Shop, bool>> where)
        {
            return _ShopRepository.GetMany(where);
        }
    }
}
