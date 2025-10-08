namespace DCI.Core.Common
{
	public class Constants
	{
		public const string DefaultSMTP = "172.16.1.39";
		public const string DocNo = "172.16.1.39";
		public const string CompanyCode = "DCI";
		public const string AppCode = "ESS";		
        public const string SYSAD = "SYSAD";

        public const string ModuleCode_DTR = "DTR";
        public const string ModuleCode_Overtime = "OT";
        public const string ModuleCode_Leave = "RQT";
        public const string ModuleCode_Late = "LT";
        public const string ModuleCode_Late_Deduction = "LTD";
        public const string ModuleCode_Undertime = "UT";
        public const string ModuleCode_Undertime_Deduction = "UTD";
        public const string ModuleCode_WFH = "WFH";

        public const string Msg_ErrorMessage = "An error occurred. Please try again.";
        public const string Msg_NoBiometricsFound = "No biometric records found.";

        public const string Email_Subject_SetPassword = "DCI ESS - Set Password";
		public const string Email_Subject_ResetPassword = "DCI ESS - Reset Password";
		public const string Email_Subject_UploadFile = "DCI ESS - Please Upload Your Document No";
        public const string Email_Subject_BiometricAttendance = "DCI ESS - Biometric Attendance Logged";
        
        public const string Approval_Approved = "Approved";
		public const string Approval_Disapproved = "Disapproved";
		public const string Approval_Reviewed = "Reviewed";

        public const string Source_Biometrics = "BIOMETRICS";
        public const string Source_Remote = "REMOTE";
        public const string Source_Suspension = "SUSPENSION";
        public const string Source_Holiday = "HOLIDAY";


        public const string OverTime_Regular = "125% REGULAR (AFTER OFFICE HRS. /MON - FRI / EXCEPT HOLIDAY)";
        public const string OverTime_NightDifferential = "10% NIGHT DIFFERENTIAL (10PM - 6AM) MON - SUN / HOLIDAY";
        public const string OverTime_SpecialHoliday = "130% SPECIAL HOLIDAY MON - SUN / SAT-SUN (FIRST 8 HRS.)";
        public const string OverTime_After8hrs = "169% AFTER 8 HRS OF 130%";
        public const string OverTime_HolidayOnRestDay = "150% HOLIDAY ON REST DAY (SAT - SUN)";
		public const string OverTime_Requires1Hr = "Overtime filing requires at least 1 hour.";
		public const string OverTime_Requires8HrNotBeenMet = "Overtime cannot be filed for this date because the required 8 working hours have not been met.";
        public const string OverTime_ExceedsActualTimeOut = "Your filed overtime exceeds your actual time out.";

        public const string Msg_NoOfficialBusinessRecord  = "No Official Business record was found for the selected date. Please file and secure approval for Official Business before submitting an overtime request.";

        public const string Undertime_Deduction = "Deductions successfully applied to all selected employees.";
        
    }
}
