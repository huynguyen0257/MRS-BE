using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MRS.Model
{
    public class PopularProduct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        #region Relationship
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }
        #endregion
    }
}
