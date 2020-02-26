using System;
using System.Collections.Generic;

namespace MRS.ViewModels
{
    public class CartVM
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public DateTime DateCreated { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
    }

    public class CartCM //Nho lay User ra
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CartUM
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderCM
    {
        public OrderCM()
        {
            CartIds = new List<Guid>();
        }
        public List<Guid> CartIds { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public float Price { get; set; }
        public string Note { get; set; }
    }
}
