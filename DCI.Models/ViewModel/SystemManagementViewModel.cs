namespace DCI.Models.ViewModel
{
	public class SystemManagementViewModel
	{
		public int RoleId { get; set; } = 0;
		public string RoleName { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public bool Dashboard { get; set; } = false;

		public bool Form201 { get; set; } = false;
		public bool Form201View { get; set; } = false;
		public bool Form201Add { get; set; } = false;
		public bool Form201Update { get; set; } = false;
		public bool Form201Delete { get; set; } = false;
		public bool Form201Import { get; set; } = false;
		public bool Form201Export { get; set; } = false;

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

		public bool EmployeeMaster { get; set; } = false;
		public bool EmployeeMasterView { get; set; } = false;
		public bool EmployeeMasterAdd { get; set; } = false;
		public bool EmployeeMasterUpdate { get; set; } = false;
		public bool EmployeeMasterDelete { get; set; } = false;
		public bool EmployeeMasterImport { get; set; } = false;
		public bool EmployeeMasterExport { get; set; } = false;

		public bool Holiday { get; set; } = false;
		public bool HolidayView { get; set; } = false;
		public bool HolidayAdd { get; set; } = false;
		public bool HolidayUpdate { get; set; } = false;
		public bool HolidayDelete { get; set; } = false;
		public bool HolidayImport { get; set; } = false;
		public bool HolidayExport { get; set; } = false;

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

        public bool Todo { get; set; } = false;

        public bool Reports { get; set; } = false;

        public bool DTRManagement { get; set; } = false;

        public bool Attendance { get; set; } = false;
        public bool AttendanceView { get; set; } = false;
        public bool AttendanceAdd { get; set; } = false;
        public bool AttendanceUpdate { get; set; } = false;
        public bool AttendanceDelete { get; set; } = false;
        public bool AttendanceImport { get; set; } = false;
        public bool AttendanceExport { get; set; } = false;

        public bool DailyTimeRecord { get; set; } = false;
        public bool DailyTimeRecordView { get; set; } = false;
        public bool DailyTimeRecordAdd { get; set; } = false;
        public bool DailyTimeRecordUpdate { get; set; } = false;
        public bool DailyTimeRecordDelete { get; set; } = false;
        public bool DailyTimeRecordImport { get; set; } = false;
        public bool DailyTimeRecordExport { get; set; } = false;

        public bool Leave { get; set; } = false;
        public bool LeaveView { get; set; } = false;
        public bool LeaveAdd { get; set; } = false;
        public bool LeaveUpdate { get; set; } = false;
        public bool LeaveDelete { get; set; } = false;
        public bool LeaveImport { get; set; } = false;
        public bool LeaveExport { get; set; } = false;

        public bool Overtime { get; set; } = false;
        public bool OvertimeView { get; set; } = false;
        public bool OvertimeAdd { get; set; } = false;
        public bool OvertimeUpdate { get; set; } = false;
        public bool OvertimeDelete { get; set; } = false;
        public bool OvertimeImport { get; set; } = false;
        public bool OvertimeExport { get; set; } = false;

        public bool Position { get; set; } = false;
        public bool PositionView { get; set; } = false;
        public bool PositionAdd { get; set; } = false;
        public bool PositionUpdate { get; set; } = false;
        public bool PositionDelete { get; set; } = false;
        public bool PositionImport { get; set; } = false;
        public bool PositionExport { get; set; } = false;

        public bool WFH { get; set; } = false;
        public bool WFHView { get; set; } = false;
        public bool WFHAdd { get; set; } = false;
        public bool WFHUpdate { get; set; } = false;
        public bool WFHDelete { get; set; } = false;
        public bool WFHImport { get; set; } = false;
        public bool WFHExport { get; set; } = false;

        public bool Undertime { get; set; } = false;
        public bool UndertimeView { get; set; } = false;
        public bool UndertimeAdd { get; set; } = false;
        public bool UndertimeUpdate { get; set; } = false;
        public bool UndertimeDelete { get; set; } = false;
        public bool UndertimeImport { get; set; } = false;
        public bool UndertimeExport { get; set; } = false;

        public bool DTRCorrection { get; set; } = false;
        public bool DTRCorrectionView { get; set; } = false;
        public bool DTRCorrectionAdd { get; set; } = false;
        public bool DTRCorrectionUpdate { get; set; } = false;
        public bool DTRCorrectionDelete { get; set; } = false;
        public bool DTRCorrectionImport { get; set; } = false;
        public bool DTRCorrectionExport { get; set; } = false;

        public int ViewEdit { get; set; } = 1;
		public string DashboardLabel { get; set; } = string.Empty;
	}
}
