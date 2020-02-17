using MRS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRS.Data.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        MRSContext dbContext;

        public MRSContext Init()
        {
            return dbContext ?? (dbContext = new MRSContext());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}
