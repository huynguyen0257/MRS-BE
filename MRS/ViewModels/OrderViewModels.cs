using System;
using System.Collections.Generic;

namespace MRS.ViewModels
{
    public class OrderVM
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public float Price { get; set; }
        public int Status { get; set; }
    }

    public class OrderDetailVM : OrderVM
    {
        public OrderDetailVM()
        {
            CartVMs = new List<CartVM>();
        }
        public string Note { get; set; }
        public List<CartVM> CartVMs { get; set; }
    }
}
