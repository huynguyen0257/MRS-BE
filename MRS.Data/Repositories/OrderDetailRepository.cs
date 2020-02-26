using MRS.Data.Infrastructure;
using MRS.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRS.Data.Repositories
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {

    }

    public class OrderDetailRepository : RepositoryBase<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(IDbFactory dbFactory) : base(dbFactory) { }
    }
}
