namespace DCI.Models.ViewModel
{
	public class RoleInModuleViewModel
	{
		public Dictionary<string, ModuleJson> Modules { get; set; } = new Dictionary<string, ModuleJson>();
		public RoleViewModel RoleVM { get; set; } = new RoleViewModel();
		public List<ModuleInRoleViewModel> ModuleInRoleList { get; set; } = new List<ModuleInRoleViewModel>();
	}


	public class ModuleJson
	{
		public int RoleId { get; set; } = 0;
		public string RoleName { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;	
		public string MainModule { get; set; } = string.Empty;// This stores the main module ID as a string
		public List<string> SubModules { get; set; } = new List<string>();   // This stores submodules as a list of string IDs
	}
}