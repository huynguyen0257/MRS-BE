using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MRS.ViewModels
{
    public class CategoryVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    public class CategoryCM
    {
        public string Name { get; set; }
    }
    public class CategoryUM : CategoryVM
    {
    }
}
