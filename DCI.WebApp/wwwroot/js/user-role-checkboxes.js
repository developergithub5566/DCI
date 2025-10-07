// Check All checkboxes

let checkAllBox_EmployeeMaster = document.getElementById('chkAllEmployeeMaster');
checkAllBox_EmployeeMaster.addEventListener('change', function () {
	let checkboxes = document.querySelectorAll('.chk_EmployeeMaster');
	checkboxes.forEach((checkbox) => {
		checkbox.checked = checkAllBox_EmployeeMaster.checked;
		checkbox.disabled = !checkAllBox_EmployeeMaster.checked;
	});
});

let checkAllBox_UserManagement = document.getElementById('chkAllUserManagement');
checkAllBox_UserManagement.addEventListener('change', function () {
	let checkboxes = document.querySelectorAll('.user_management');
	checkboxes.forEach((checkbox) => {
		checkbox.checked = checkAllBox_UserManagement.checked;
		checkbox.disabled = !checkAllBox_UserManagement.checked;
	});
});

let checkAllBox_Department = document.getElementById('chkAllDepartment');
checkAllBox_Department.addEventListener('change', function () {
	let checkboxes = document.querySelectorAll('.chk_departments');
	checkboxes.forEach((checkbox) => {
		checkbox.checked = checkAllBox_Department.checked;
		checkbox.disabled = !checkAllBox_Department.checked;
	});
});

let checkAllBox_Holiday = document.getElementById('chkAllHoliday');
checkAllBox_Holiday.addEventListener('change', function () {
	let checkboxes = document.querySelectorAll('.chk_holiday');
	checkboxes.forEach((checkbox) => {
		checkbox.checked = checkAllBox_Holiday.checked;
		checkbox.disabled = !checkAllBox_Holiday.checked;
	});
});

let checkAllBox_Announcement = document.getElementById('chkAllAnnouncement');
checkAllBox_Announcement.addEventListener('change', function () {
	let checkboxes = document.querySelectorAll('.chk_announcement');
	checkboxes.forEach((checkbox) => {
		checkbox.checked = checkAllBox_Announcement.checked;
		checkbox.disabled = !checkAllBox_Announcement.checked;
	});
});

let checkAllBox_Position = document.getElementById('chkAllPosition');
checkAllBox_Position.addEventListener('change', function () {
	let checkboxes = document.querySelectorAll('.chk_position');
	checkboxes.forEach((checkbox) => {
		checkbox.checked = checkAllBox_Position.checked;
		checkbox.disabled = !checkAllBox_Position.checked;
	});
});

let checkAllBox_AuditTrail = document.getElementById('chkAllAuditTrail');
checkAllBox_AuditTrail.addEventListener('change', function () {
	let checkboxes = document.querySelectorAll('.chk_auditTrail');
	checkboxes.forEach((checkbox) => {
		checkbox.checked = checkAllBox_AuditTrail.checked;
		checkbox.disabled = !checkAllBox_AuditTrail.checked;
	});
});

let checkAllBox_UserRoleManagement = document.getElementById('chkAllUserRoleManagement');
checkAllBox_UserRoleManagement.addEventListener('change', function () {
	let checkboxes = document.querySelectorAll('.chk_userRoleManagement');
	checkboxes.forEach((checkbox) => {
		checkbox.checked = checkAllBox_UserRoleManagement.checked;
		checkbox.disabled = !checkAllBox_UserRoleManagement.checked;
	});
});

let checkAllBox_Form201 = document.getElementById('chkAllform201');
checkAllBox_Form201.addEventListener('change', function () {
	let checkboxes = document.querySelectorAll('.chk_form201');
	checkboxes.forEach((checkbox) => {
		checkbox.checked = checkAllBox_Form201.checked;
		checkbox.disabled = !checkAllBox_Form201.checked;
	});
});
