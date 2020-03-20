using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MRS.ViewModels
{
    public class ProductVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string MainImage { get; set; }
        public int NumberOfLike { get; set; }
    }
    
    public class ProductDetailVM : ProductUM // Đuọc xem khong duoc Update
    {
        public int NumberOfLike { get; set; }
        public int Avaiable { get; set; }
        public int Ordered { get; set; }
        public int Purchased { get; set; }
        public bool IsLiked { get; set; }
        public List<string> Images { get; set; }
        public DateTime DateCreated { get; set; }

    }

    public class ProductCM
    {
        public string Name { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
    }
    public class ProductUM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public Guid CategoryId { get; set; }
    }
}
