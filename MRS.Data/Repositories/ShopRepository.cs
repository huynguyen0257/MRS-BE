using MRS.Data.Infrastructure;
using MRS.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRS.Data.Repositories
{
    public interface IShopRepository : IRepository<Shop>
    {

    }

    public class ShopRepository : RepositoryBase<Shop>, IShopRepository
    {
        public ShopRepository(IDbFactory dbFactory) : base(dbFactory) { }
    }
}
