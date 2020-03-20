using MRS.Data.Infrastructure;
using MRS.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRS.Data.Repositories
{
    public interface IPopularProductRepository : IRepository<PopularProduct>
    {

    }

    public class PopularProductRepository : RepositoryBase<PopularProduct>, IPopularProductRepository
    {
        public PopularProductRepository(IDbFactory dbFactory) : base(dbFactory) { }
    }
}
