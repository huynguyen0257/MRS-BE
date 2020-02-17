using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MRS.Model
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public int NumberOfLike { get; set; }
        public bool IsDelete { get; set; }
        public DateTime DateCreated { get; set; }
        public string UserCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string UserUpdated { get; set; }

        #region Relationship
        public virtual ICollection<PopularProduct> PopularProducts { get; set; }
        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual WareHouse WareHouse { get; set; }
        #endregion
    }
}
