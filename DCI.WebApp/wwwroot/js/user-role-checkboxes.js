// Check All checkboxes

//let checkAllBox_Document = document.getElementById('chkAllDocument');
//checkAllBox_Document.addEventListener('change', function () {
//	let checkboxes = document.querySelectorAll('.chk_auditTrail');
//	checkboxes.forEach((checkbox) => {
//		checkbox.checked = checkAllBox_Document.checked;
//		checkbox.disabled = !checkAllBox_Document.checked;
//	});
//});

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

//let checkAllBox_Section = document.getElementById('chkAllSection');
//checkAllBox_Section.addEventListener('change', function () {
//	let checkboxes = document.querySelectorAll('.chk_section');
//	checkboxes.forEach((checkbox) => {
//		checkbox.checked = checkAllBox_Section.checked;
//		checkbox.disabled = !checkAllBox_Section.checked;
//	});
//});

//let checkAllBox_Documenttype = document.getElementById('chkAllDocumenttype');
//checkAllBox_Documenttype.addEventListener('change', function () {
//	let checkboxes = document.querySelectorAll('.chk_documenttype');
//	checkboxes.forEach((checkbox) => {
//		checkbox.checked = checkAllBox_Documenttype.checked;
//		checkbox.disabled = !checkAllBox_Documenttype.checked;
//	});
//});

//let checkAllBox_Announcement = document.getElementById('chkAllAnnouncement');
//checkAllBox_Announcement.addEventListener('change', function () {
//	let checkboxes = document.querySelectorAll('.chk_announcement');
//	checkboxes.forEach((checkbox) => {
//		checkbox.checked = checkAllBox_Announcement.checked;
//	});
//});

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
