using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MRS.Model
{
    public class WareHouse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public int Avaiable { get; set; }
        public int Ordered { get; set; }
        public int Purchased { get; set; }

        #region Relationship
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        #endregion
    }
}
