using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MRS.Model
{
    public class DeliveryData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsDefault { get; set; }

        #region Relationship
        public string UserId { get; set; }
        public virtual User User { get; set; }
        #endregion
    }
}
