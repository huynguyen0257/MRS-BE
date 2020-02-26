using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BpmnKit.BpmnViewModels
{
    public class LoginMobiVM
    {
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        public string Account_Id { get; set; }
    }
    public class Token
    {
        public string[] roles { get; set; }
        public string fullname { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }

    public class RegisterMobiVM
    {
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Account_Id { get; set; }// < =========
        public string Device_Id { get; set; }

    }
}
