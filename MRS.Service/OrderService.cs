using MRS.Data.Infrastructure;
using MRS.Data.Repositories;
using MRS.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MRS.Service
{
    public interface IOrderService
    {
        IEnumerable<Order> GetOrders();
        IEnumerable<Order> GetOrders(Expression<Func<Order, bool>> where);
        Order GetOrder(Guid id);
        void CreateOrder(Order Order, string username);
        void EditOrder(Order Order, string username);
        void RemoveOrder(Order Order);
        void SaveOrder();
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _OrderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository, IUnitOfWork unitOfWork)
        {
            _OrderRepository = orderRepository;
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
        }

        public void CreateOrder(Order Order, string username)
        {
            //Check inventory
            var carts = Order.Carts;
            string error = "";
            float price = 0;
            foreach (var cart in carts)
            {
                var warehouse = cart.WareHouse;
                if (warehouse.Avaiable < cart.Quantity) error += cart.ProductName + "Just have " + warehouse.Avaiable + " items\n";
                price += cart.Price;
                
            }
            if (!String.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }

            
            Order.Status = Int32.Parse(OrderStatus.processing.ToString());
            Order.Price = price;
            Order.DateCreated = DateTime.Now;
            Order.UserCreated = username;
            _OrderRepository.Add(Order);

            //update cart
            foreach (var cart in carts)
            {
                cart.Status = Int32.Parse(CartStatus.ordered.ToString());
                cart.UserUpdated = username;
                cart.DateUpdated = DateTime.Now;
                AutoUpdateWarehouse(cart.WareHouse,null, cart.Quantity, null);
                _cartRepository.Update(cart);
            }
        }

        public void EditOrder(Order Order, string username)
        {
            Order.DateUpdated = DateTime.Now;
            Order.UserUpdated = username;
            _OrderRepository.Update(Order);
        }

        public Order GetOrder(Guid id)
        {
            return _OrderRepository.GetById(id);
        }

        public IEnumerable<Order> GetOrders()
        {
            return _OrderRepository.GetAll();
        }

        public void SaveOrder()
        {
            _unitOfWork.Commit();
        }

        public void RemoveOrder(Order Order)
        {
            _OrderRepository.Delete(Order);
        }

        public IEnumerable<Order> GetOrders(Expression<Func<Order, bool>> where)
        {
            return _OrderRepository.GetMany(where);
        }

        /// <summary>
        /// hello
        /// </summary>
        /// <param name="wareHouse"></param>
        /// <param name="Avaiable">huỷ đặt hàng</param>
        /// <param name="Ordered">Đặt hàng</param>
        /// <param name="Purchased">Thanh toan</param>
        /// <param name="increaseQuantity">Update so luong</param>
        private void AutoUpdateWarehouse(WareHouse wareHouse, int? returnQuantity, int? orderQuantity, int? purchaseQuantity)
        {
            if (returnQuantity != null)  //tra đặt hàng
            {
                wareHouse.Avaiable = wareHouse.Avaiable + returnQuantity.Value;
                wareHouse.Ordered = wareHouse.Ordered - returnQuantity.Value;
            }
            else if (orderQuantity != null) //Đặt hàng
            {
                wareHouse.Ordered = wareHouse.Ordered + orderQuantity.Value;
                wareHouse.Avaiable = wareHouse.Avaiable - orderQuantity.Value;
            }
            else if (purchaseQuantity != null) //Thanh toan
            {
                wareHouse.Purchased = wareHouse.Purchased + purchaseQuantity.Value;
                wareHouse.Ordered = wareHouse.Ordered - purchaseQuantity.Value;
            }
        }
    }
}
