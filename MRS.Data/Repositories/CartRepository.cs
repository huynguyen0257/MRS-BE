using MRS.Data.Infrastructure;
using MRS.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRS.Data.Repositories
{
    public interface ICartRepository : IRepository<Cart>
    {

    }

    public class CartRepository : RepositoryBase<Cart>, ICartRepository
    {
        public CartRepository(IDbFactory dbFactory) : base(dbFactory) { }
    }
}
