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
        Form201 = 3,
        Administration = 4,
        UserManagement = 5,
        Department = 6,
        EmployeeMaster = 7,
        Holiday = 8,
        UserRole = 9,
        SystemManagement = 10,
        AuditTrail = 11,
        Announcement = 12,
        Todo = 13,
        Reports = 14,
        DTRManagement = 15,
        Attendance = 16,
        DailyTimeRecord = 17,
        [Description("Leave")]
        Leave = 18,
        [Description("Overtime")]
        Overtime = 19,
        Position = 20,
        [Description("WFH")]
        WFH = 21,
        [Description("Undertime")]
        Undertime = 22,
        [Description("DTR Adjustment")]
        DTRCorrection = 23,
        Late = 24,
        TriggerLeaveCreditProcess = 25,
        LeaveManagement = 26
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
        //IMPORTANT ORDERING FOR Leave Computaion Stored Procedure : get_LeaveBalance
        Regular = 1,
        Probationary = 2,
        Contractual = 3,
        Resigned = 4,
        FixedTerm = 5,
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
        //IMPORTANT ORDERING FOR Leave Computaion Stored Procedure : get_LeaveBalance
        Raw = 0,
        Draft = 1,
        Pending = 2,
        ForApproval = 3,
        Approved = 4,
        Rejected = 5,
        Cancelled = 6,
        Deleted = 7,
        Active = 8,
        PayrollDeducted = 9,
        VLDeducted = 10,
        DTRAdjustment = 11
    }

    public enum EnumLeaveType
    {
        //IMPORTANT ORDERING FOR Leave Computaion Stored Procedure : get_LeaveBalance and Leave Module
        //HD = 1,
        //VL = 2,
        //SL = 3,
        //SPL = 4,
        //ML = 5,
        //PL = 6,
        //OB = 7,
        //VLMon = 8,
        //SLMon = 9,
        //UT = 10,
        //LT = 11
        HDVL = 1,
        HDSL = 2,
        VL = 3,
        SL = 4,
        SPL = 5,
        ML = 6,
        PL = 7,
        OB = 8,
        VLMon = 9,
        SLMon = 10,
        UT = 11,
        LT = 12,
        OF = 13, //For OLD ESS Migration
        WFH = 14, //For OLD ESS Migration
        HDOB = 15
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

    public enum EnumHoliday
    {
        Regular = 1,
        Special = 2,
        Suspension = 3, 
    }

    public enum EnumDeductionType
    {
        [Description("Payroll")]
        Payroll = 1,
        VacationLeave = 2,
        SickLeave = 3,
        SpecialLeave = 4
    }

    public enum EnumSource
    {
        BIOMETRICS = 1,
        REMOTE = 2,
        HOLIDAY = 3,
        SUSPENSION = 4
    }
    //public enum EnumBiometricConfirmationType
    //{
    //    NoAttendance = 1,
    //    HalfdayOB = 2,
    //    NoTimeIn = 3,
    //    NoTimeOut = 4
    //}
    //
    public enum EnumScopeTypeJobRecurring
    {
        DAILY = 1,
        MONTHLY = 2    
    }

    public enum EnumRequestFiledBy
    {
        Self = 0,
        HR = 1,
        Admin = 2
    }
}
