using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MRS.Utils
{
    public enum UserRoles
    {
        [Display(Name = "Quản lý")]
        Admin = 0,
        [Display(Name = "Khách hàng")]
        Customer = 5,
    }

    public static class RolesExtenstions
    {
        public static async Task InitAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (string roleName in Enum.GetNames(typeof(UserRoles)))
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
