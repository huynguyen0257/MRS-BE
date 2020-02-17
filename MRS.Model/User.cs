using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRS.Model
{
    public class User : IdentityUser
    {
        public String FullName { get; set; }
        public string QRCode { get; set; }
        public int Level { get; set; }

        #region relationship
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<DeliveryData> DeliveryDatas { get; set; }
        public virtual ICollection<PopularProduct> PopularProducts { get; set; }
        #endregion
    }
    public enum UserLevel
    {
        Member,
        Gold,
        Platinum
    }
}
