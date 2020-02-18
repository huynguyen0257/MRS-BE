using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MRS.ViewModels
{
    public class ShopVM
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string ActiveTime { get; set; }
    }
    public class ShopDetailVM : ShopUM
    {
        public DateTime DateCreated { get; set; }
    }

    public class ShopCM
    {
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string ActiveTime { get; set; }
    }

    public class ShopUM
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string ActiveTime { get; set; }
    }


}
