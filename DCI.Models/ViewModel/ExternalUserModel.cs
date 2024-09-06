using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Models.ViewModel
{
	public class ExternalUserModel
	{
		public string Identifier { get; set; } = string.Empty;
		public string Firstname { get; set; } = string.Empty;
		public string Lastname { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Provider { get; set; } = string.Empty;
	}
}
