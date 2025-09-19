namespace DCI.Core.Common
{
	public class Constants
	{
		public const string DefaultSMTP = "172.16.1.39";
		public const string DocNo = "172.16.1.39";
		public const string CompanyCode = "DCI";
		public const string AppCode = "DMS";
		public const string DocControlNo = "CN";

		public const string Msg_ErrorMessage = "An error occurred. Please try again.";
        public const string Msg_NoBiometricsFound = "No biometric records found.";

        public const string Email_Subject_SetPassword = "DCI App - Set Password";
		public const string Email_Subject_ResetPassword = "DCI App - Reset Password";
		public const string Email_Subject_UploadFile = "DCI App - Please Upload Your Document No";

		public const string Approval_Approved = "Approved";
		public const string Approval_Disapproved = "Disapproved";
		public const string Approval_Reviewed = "Reviewed";


		public const string OverTime_Regular = "125% REGULAR (AFTER OFFICE HRS. /MON - FRI / EXCEPT HOLIDAY";
        public const string OverTime_NightDifferential = "10% NIGHT DIFFERENTIAL (10PM - 6AM) MON - SUN / HOLIDAY";
        public const string OverTime_SpecialHoliday = "130% SPECIAL HOLIDAY MON - SUN / SAT-SUN (FIRST 8 HRS.)";
        public const string OverTime_After8hrs = "169% AFTER 8 HRS OF 130%";
        public const string OverTime_HolidayOnRestDay = "150% HOLIDAY ON REST DAY (SAT - SUN)";
		public const string OverTime_Requires1Hr = "Overtime filing requires at least 1 hour.";
		public const string OverTime_Requires8HrNotBeenMet = "Overtime cannot be filed for this date because the required 8 working hours have not been met.";

        public const string Undertime_Deduction = "Deductions successfully applied to all selected employees.";
        
    }
}
