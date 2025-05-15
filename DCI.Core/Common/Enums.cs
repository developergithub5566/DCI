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
	//public enum EnumStatusType
	//{
	//	Draft = 1,
	//	ForReview = 2,
	//	ForApproval = 3,
	//	Approved = 4,
	//	ForDisposal = 5,
	//	Disposed = 6
	//}

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
		Announcement = 12 ,
		Todo = 13,
		Reports = 14,
		Form201 = 15,
        DTRManagement = 16,
        DailyTimeRecord = 17
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
		Draft = 1,
		Pending = 2,
		InProgress = 3,
		ForReview = 4,
		Reviewed = 5,
		ForApproval =6,
		Approved = 7,
		Deleted = 8,
		Rejected = 9,
	}
	public enum EnumDocumentCategory
	{
		[Description("Internal")]
		Internal = 1,
		[Description("Internal/External")] 
		BothInExternal = 2
	}


	public enum EnumEmploymentType
	{
		Regular = 1,
        Probationary = 2,
		Contractual = 3,
		FixedTerm = 4,
        Resigned = 5,
        AWOL = 6,
    }

    public enum EnumJobPosition
    {
       DepartmentHead = 1,
	   Finance = 2,
	   SoftwareEngineer = 3	
    }

    public enum EnumSex
    {
        Male = 1,
        Female = 2,      
    }

    public enum EnumStatus
    {
        Draft = 0,
        Save = 1,
        ForApproval = 2,   
		Approved = 3,
		Deleted = 4,
		
    }

}
