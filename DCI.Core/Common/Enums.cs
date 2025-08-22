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
        Announcement = 12,
        Todo = 13,
        Reports = 14,
        Form201 = 15,
        DTRManagement = 16,
        DailyTimeRecord = 17,
        Leave = 18,
        Overtime = 19,
        Holiday = 20,
        WFH = 21
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
        ForApproval = 6,
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
        Draft = 1,
        Pending = 2,
        ForApproval = 3,
        Approved = 4,
        Rejected = 5,
        Cancelled = 6,
        Deleted = 7,
        Active = 8,
        InActive = 9
    }

    public enum EnumLeaveType
    {
        HD = 1,
        VL = 2,
        SL = 3,
        SPL = 4,
        ML = 5,
        PL = 6,
        MS = 7,
        OB = 8,
        VLMon = 9,
        SLMon = 10
    }

    public enum EnumEmployeeScope
    {
        ALL = 0,
        PerEmployee = 1,
    }

    public enum EnumOvertime
    {
        Regular = 1,
        NightDifferential = 2,
        SpecialHoliday = 3,
        After8hrs = 4,
        HolidayOnRestDay = 5
    }
}
