namespace DCI.Models.ViewModel
{
	public class UserInRoleViewModel
	{
		public int RoleId { get; set; } = 0;
		public string RoleName { get; set; } = string.Empty;
		public int UserCount { get; set; } = 0;
		public int ModuleCount { get; set; } = 0;
		public int SubModuleCount { get; set; } = 0;
		public int ModifiedBy { get; set; } = 0;
	}
}
