using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MRS.ViewModels
{
    public class SoldProductVM
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
    
    public class InventoryVM
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
    
    public class SalePriceVM
    {
        public float SalePrice { get; set; }
        public float InventoryPrice { get; set; }
        public float TotalPrice { get; set; }
    }
}
