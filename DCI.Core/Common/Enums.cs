using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Core.Common
{
	public enum EnumUserAccess
	{
		Administrator = 1,
		User = 2
	}
	public enum EnumStatusType
	{
		Draft = 1,
		ForReview = 2,
		ForApproval = 3,
		Approved = 4,
		ForDisposal = 5,
		Disposed = 6
	}

	public enum EnumModulePage
	{
		Login = 1,
		Dashboard = 2,
		Document = 3,
		Administration = 4,
		UserManagement = 5,
		Department = 6,
		Section = 7,
		DocumentType = 8,
		UserRole = 9,
		SystemManagement = 10,
		AuditTrail = 11,
		Announcement = 12 
	}
	public enum EnumRole
	{
		Admin = 1,
		User = 2
	}

	public enum EnumPermissionRole
	{
		View = 1,
		Add = 2,
		Update = 3,
		Delete = 4,
		Import = 5,
		Export = 6
	}

	public enum EnumApplicantStatus
	{
		Draft = 0,
		Save = 1,
		[Description("Sent Invitation")] //or for interview
		ForInitialInterview = 2,
		[Description("Done Initial Interview")]// passed initial interview 
		DoneInitialInterview = 3,
		ForFinalInterview = 4,
		PassedFinalInterview = 5,
		ForRequirements = 6,
		Withdraw = 7,
	}
	public enum EnumDocumentStatus
	{
		Draft = 0,
		Save = 1
	}
	public enum EnumDocumentCategory
	{
		[Description("Internal")]
		Internal = 1,
		[Description("Internal/External")] 
		BothInExternal = 2
	}
	public enum EnumDocumentSection
	{ 	
		[Description("Project Management")]
		PM = 0,
		[Description("HR")]
		HR = 1
	}

	public enum EnumLabel
	{
		Forms = 1,
		Process = 2
	}

	public enum EnumLabelCode
	{
		F = 1, //Forms = 1,
		P = 2 //Process = 2
	}

	//public enum EnumCategoryCode
	//{
	//	TID = 1,
	//	HR = 2,
	//	Finance = 3
	//}
	//public enum EnumSectionCode
	//{
	//	PM =1,
	//	HR = 2,
	//}
}
