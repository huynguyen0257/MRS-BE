using MRS.Data.Infrastructure;
using MRS.Data.Repositories;
using MRS.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MRS.Service
{
    public interface IOrderDetailService
    {
        IEnumerable<OrderDetail> GetOrderDetails();
        IEnumerable<OrderDetail> GetOrderDetails(Expression<Func<OrderDetail, bool>> where);
        OrderDetail GetOrderDetail(Guid id);
        void CreateOrderDetail(OrderDetail OrderDetail, string username);
        void EditOrderDetail(OrderDetail OrderDetail, string username);
        void RemoveOrderDetail(OrderDetail OrderDetail);
        void SaveOrderDetail();
    }

    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _OrderDetailRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderDetailService(IOrderDetailRepository OrderDetailRepository, IUnitOfWork unitOfWork)
        {
            this._OrderDetailRepository = OrderDetailRepository;
            this._unitOfWork = unitOfWork;
        }

        public void CreateOrderDetail(OrderDetail OrderDetail, string username)
        {
            OrderDetail.DateCreated = DateTime.Now;
            OrderDetail.UserCreated = username;
            _OrderDetailRepository.Add(OrderDetail);
        }

        public void EditOrderDetail(OrderDetail OrderDetail, string username)
        {
            OrderDetail.DateUpdated = DateTime.Now;
            OrderDetail.UserUpdated = username;
            _OrderDetailRepository.Update(OrderDetail);
        }

        public OrderDetail GetOrderDetail(Guid id)
        {
            return _OrderDetailRepository.GetById(id);
        }

        public IEnumerable<OrderDetail> GetOrderDetails()
        {
            return _OrderDetailRepository.GetAll();
        }

        public void SaveOrderDetail()
        {
            _unitOfWork.Commit();
        }

        public void RemoveOrderDetail(OrderDetail OrderDetail)
        {
            _OrderDetailRepository.Delete(OrderDetail);
        }

        public IEnumerable<OrderDetail> GetOrderDetails(Expression<Func<OrderDetail, bool>> where)
        {
            return _OrderDetailRepository.GetMany(where);
        }
    }
}
