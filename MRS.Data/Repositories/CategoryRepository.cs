using MRS.Data.Infrastructure;
using MRS.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRS.Data.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {

    }

    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(IDbFactory dbFactory) : base(dbFactory) { }
    }
}
