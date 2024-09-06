// Check All checkboxes
let checkAllBox_UserManagement = document.getElementById('chkAllUserManagement');
checkAllBox_UserManagement.addEventListener('change', function () {
	let checkboxes = document.querySelectorAll('.user_management');
	checkboxes.forEach((checkbox) => {
		checkbox.checked = checkAllBox_UserManagement.checked;
	});
});

let checkAllBox_Department = document.getElementById('chkAllDepartment');
checkAllBox_Department.addEventListener('change', function () {
	let checkboxes = document.querySelectorAll('.chk_departments');
	checkboxes.forEach((checkbox) => {
		checkbox.checked = checkAllBox_Department.checked;
	});
});

let checkAllBox_Employmenttype = document.getElementById('chkAllEmploymenttype');
checkAllBox_Employmenttype.addEventListener('change', function () {
	let checkboxes = document.querySelectorAll('.chk_employmenttype');
	checkboxes.forEach((checkbox) => {
		checkbox.checked = checkAllBox_Employmenttype.checked;
	});
});

let checkAllBox_Announcement = document.getElementById('chkAllAnnouncement');
checkAllBox_Announcement.addEventListener('change', function () {
	let checkboxes = document.querySelectorAll('.chk_announcement');
	checkboxes.forEach((checkbox) => {
		checkbox.checked = checkAllBox_Announcement.checked;
	});
});

let checkAllBox_AuditTrail = document.getElementById('chkAllAuditTrail');
checkAllBox_AuditTrail.addEventListener('change', function () {
	let checkboxes = document.querySelectorAll('.chk_auditTrail');
	checkboxes.forEach((checkbox) => {
		checkbox.checked = checkAllBox_AuditTrail.checked;
	});
});

let checkAllBox_UserRoleManagement = document.getElementById('chkAllUserRoleManagement');
checkAllBox_UserRoleManagement.addEventListener('change', function () {
	let checkboxes = document.querySelectorAll('.chk_userRoleManagement');
	checkboxes.forEach((checkbox) => {
		checkbox.checked = checkAllBox_UserRoleManagement.checked;
	});
});


