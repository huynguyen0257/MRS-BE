using MRS.Data.Infrastructure;
using MRS.Data.Repositories;
using MRS.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MRS.Service
{
    public interface ICartService
    {
        IEnumerable<Cart> GetCarts();
        IEnumerable<Cart> GetCarts(Expression<Func<Cart, bool>> where);
        Cart GetCart(Guid id);
        void CreateCart(Cart Cart, string username);
        void EditCart(Cart Cart, string username, int oldQuantity);
        void RemoveCart(Cart Cart);
        void RemoveCart(Guid id);
        void SaveCart();
    }

    public class CartService : ICartService
    {
        private readonly ICartRepository _CartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IWareHouseRepository _wareHouseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository, IWareHouseRepository wareHouseRepository, IUnitOfWork unitOfWork)
        {
            _CartRepository = cartRepository;
            _productRepository = productRepository;
            _wareHouseRepository = wareHouseRepository;
            _unitOfWork = unitOfWork;
        }

        public void CreateCart(Cart Cart, string username)
        {
            var warehouse = Cart.WareHouse;
            if (warehouse == null) throw new Exception("Chua tim thay product");
            if (Cart.Quantity > warehouse.Avaiable) throw new Exception("Hang trong kho khong du");
            //AutoUpdateWarehouse(warehouse, null, Cart.Quantity, null, null);
            //_wareHouseRepository.Update(warehouse);
            AutoUpdateValue(Cart);
            Cart.UserCreated = username;
            Cart.DateCreated = DateTime.Now;
            _CartRepository.Add(Cart);
        }

        public void EditCart(Cart Cart, string username, int oldQuantity)
        {
            var warehouse = Cart.WareHouse;
            if (Cart.Quantity > oldQuantity && Cart.Quantity - oldQuantity > warehouse.Avaiable)
                throw new Exception("Hang trong kho khong du");
            Cart.Price = _productRepository.GetById(Cart.ProductId).Price * Cart.Quantity;
            //var increaseQuantity = Cart.Quantity - oldQuantity;
            //AutoUpdateWarehouse(warehouse, null, null, null, increaseQuantity);
            Cart.UserUpdated = username;
            Cart.DateUpdated = DateTime.Now;
            _CartRepository.Update(Cart);
        }

        public Cart GetCart(Guid id)
        {
            return _CartRepository.GetById(id);
        }

        public IEnumerable<Cart> GetCarts()
        {
            return _CartRepository.GetAll();
        }

        public void SaveCart()
        {
            _unitOfWork.Commit();
        }

        public void RemoveCart(Cart Cart)
        {
            _CartRepository.Delete(Cart);
        }

        public IEnumerable<Cart> GetCarts(Expression<Func<Cart, bool>> where)
        {
            return _CartRepository.GetMany(where);
        }

        private void AutoUpdateValue(Cart Cart)
        {
            if (Cart.WareHouse == null) throw new Exception("WareHouse null!");
            var product = Cart.WareHouse.Product;
            Cart.Price = product.Price * Cart.Quantity;
            Cart.ProductId = product.Id;
            Cart.ProductName = product.Name;
        }

        public void RemoveCart(Guid id)
        {
            var cart = _CartRepository.GetById(id);
            _CartRepository.Delete(cart);
        }
    }
}
