using DCI.Models.Entities;

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
		//   public RolexViewModel RoleVM { get; set; } = new RolexViewModel();
		public string MainModule { get; set; } = string.Empty;// This stores the main module ID as a string
		public List<string> SubModules { get; set; } = new List<string>();   // This stores submodules as a list of string IDs
	}

	public class RolexViewModel
	{
		public int RoleId { get; set; } = 0;
		public string RoleName { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
	}

	public class SystemManagementViewModel
	{
		public int RoleId { get; set; } = 0;
		public string RoleName { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
        public bool Dashboard { get; set; } = false;
		public bool Document { get; set; } = false;
		public bool DocumentView { get; set; } = false;
		public bool DocumentAdd { get; set; } = false;
		public bool DocumentUpdate { get; set; } = false;
		public bool DocumentDelete { get; set; } = false;
		public bool DocumentImport { get; set; } = false;
		public bool DocumentExport { get; set; } = false;
		public bool Administration { get; set; } = false;
		public bool UserManagement { get; set; } = false;
		public bool UserManagementView { get; set; } = false;
		public bool UserManagementAdd { get; set; } = false;
		public bool UserManagementUpdate { get; set; } = false;
		public bool UserManagementDelete { get; set; } = false;
		public bool UserManagementImport { get; set; } = false;
		public bool UserManagementExport { get; set; } = false;

		public bool Department { get; set; } = false;
		public bool DepartmentView { get; set; } = false;
		public bool DepartmentAdd { get; set; } = false;
		public bool DepartmentUpdate { get; set; } = false;
		public bool DepartmentDelete { get; set; } = false;
		public bool DepartmentImport { get; set; } = false;
		public bool DepartmentExport { get; set; } = false;

		public bool Section { get; set; } = false;
		public bool SectionView { get; set; } = false;
		public bool SectionAdd { get; set; } = false;
		public bool SectionUpdate { get; set; } = false;
		public bool SectionDelete { get; set; } = false;
		public bool SectionImport { get; set; } = false;
		public bool SectionExport { get; set; } = false;
		public bool DocumentType { get; set; } = false;
		public bool DocumentTypeView { get; set; } = false;
		public bool DocumentTypeAdd { get; set; } = false;
		public bool DocumentTypeUpdate { get; set; } = false;
		public bool DocumentTypeDelete { get; set; } = false;
		public bool DocumentTypeImport { get; set; } = false;
		public bool DocumentTypeExport { get; set; } = false;

		public bool SystemManagement { get; set; } = false;
		public bool SystemManagementView { get; set; } = false;
		public bool SystemManagementAdd { get; set; } = false;
		public bool SystemManagementUpdate { get; set; } = false;
		public bool SystemManagementDelete { get; set; } = false;
		public bool SystemManagementImport { get; set; } = false;
		public bool SystemManagementExport { get; set; } = false;
		public bool AuditTrail { get; set; } = false;
		public bool AuditTrailView { get; set; } = false;
		public bool AuditTrailAdd { get; set; } = false;
		public bool AuditTrailUpdate { get; set; } = false;
		public bool AuditTrailDelete { get; set; } = false;
		public bool AuditTrailImport { get; set; } = false;
		public bool AuditTrailExport { get; set; } = false;

		public bool UserRoleManagement { get; set; } = false;
		public bool UserRoleManagementView { get; set; } = false;
		public bool UserRoleManagementAdd { get; set; } = false;
		public bool UserRoleManagementUpdate { get; set; } = false;
		public bool UserRoleManagementDelete { get; set; } = false;
		public bool UserRoleManagementImport { get; set; } = false;
		public bool UserRoleManagementExport { get; set; } = false;


		public bool Announcement { get; set; } = false;
		public bool AnnouncementView { get; set; } = false;
		public bool AnnouncementAdd { get; set; } = false;
		public bool AnnouncementUpdate { get; set; } = false;
		public bool AnnouncementDelete { get; set; } = false;
		public bool AnnouncementImport { get; set; } = false;
		public bool AnnouncementExport { get; set; } = false;

	}

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