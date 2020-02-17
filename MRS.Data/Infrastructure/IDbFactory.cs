using System;
using System.Collections.Generic;
using System.Text;

namespace MRS.Data.Infrastructure
{
    public interface IDbFactory : IDisposable
    {
        MRSContext Init();
    }
}
