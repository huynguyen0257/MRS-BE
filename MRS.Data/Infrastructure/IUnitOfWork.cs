using System;
using System.Collections.Generic;
using System.Text;

namespace MRS.Data.Infrastructure
{
    public interface IUnitOfWork
    {
        void Commit();
    }
}
