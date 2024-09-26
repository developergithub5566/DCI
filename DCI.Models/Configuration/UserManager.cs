using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Models.Configuration
{
    public class UserManager
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
		public string Middlename { get; set; }
		public string Lastname { get; set; }
        public string Email { get; set; }
        public string Identifier { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
		public int RoleId { get; set; }
		public string Rolename { get; set; }
        public string Fullname => $"{Firstname} {Lastname}";

		public string GetFullname()
		{
			return $"{Firstname} {Lastname}";
		}
	}
}
