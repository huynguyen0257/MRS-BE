using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MRS.ViewModels
{
    public class ProductVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public int NumberOfLike { get; set; }
    }
    
    public class ProductDetailVM : ProductUM
    {
        public int NumberOfLike { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class ProductCM
    {
        public string Name { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public int NumberOfLike { get; set; }
        public Guid CategoryId { get; set; }
        public DateTime DateCreated { get; set; }
    }
    public class ProductUM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
    }
}
