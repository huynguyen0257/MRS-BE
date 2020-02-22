using MRS.Data.Infrastructure;
using MRS.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRS.Data.Repositories
{
    public interface IWareHouseRepository : IRepository<WareHouse>
    {

    }

    public class WareHouseRepository : RepositoryBase<WareHouse>, IWareHouseRepository
    {
        public WareHouseRepository(IDbFactory dbFactory) : base(dbFactory) { }
    }
}
