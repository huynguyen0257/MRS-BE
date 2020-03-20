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
        public string UserId { get; set; }
        public float Price { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class OrderDetailVM : OrderVM
    {
        public OrderDetailVM()
        {
            CartVMs = new List<CartVM>();
        }
        public string Note { get; set; }
        public string Device_Id { get; set; }
        public List<CartVM> CartVMs { get; set; }
    }

    public class OrderDetailsVM
    {
        public int Quantity { get; set; }
        public float Price { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
        public string UserCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string UserUpdated { get; set; }

        public Guid ProductId { get; set; }
        public string ProductName { get; set; }

    }
}
