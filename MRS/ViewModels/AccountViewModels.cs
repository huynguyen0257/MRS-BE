using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MRS.ViewModels
{
	public class AccountCM
	{
		public string UserName { get; set; }
		public string FullName { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string PhoneNumber { get; set; }
		public IList<string> Roles { get; set; }
	}

    public class AccountVM
    {
        public String Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Device_Id { get; set; }
        public string Rank { get; set; }
    }
    public class AccountVMById
    {
        public String Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Device_Id { get; set; }
        public string Rank { get; set; }
        public IList<string> Roles { get; set; }
    }

    public class AccountUM
	{
		public string FullName { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string Device_Id { get; set; }
    }

    public class PasswordVM
    {
        public string PrePassword { get; set; }
        public string NewPassword { get; set; }
    }

	public class BaseRoleVM
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}

	public class BaseRoleCM
	{
		public string Name { get; set; }
	}

	public class BaseRoleUM
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}
}
