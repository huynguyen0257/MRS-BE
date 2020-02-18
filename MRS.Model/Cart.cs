﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MRS.Model
{
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public int Status { get; set; }
        public DateTime DateCreated { get; set; }
        public string UserCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string UserUpdated { get; set; }

        #region relationship
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public Guid WareHouseId { get; set; }
        public virtual WareHouse WareHouse { get; set; }
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }
        #endregion
    }
}