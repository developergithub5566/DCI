using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DCI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceController : Controller
    {
        private readonly DCIdbContext _dcIdbContext;
        IUserRepository _userRepository;
        IModulePageRepository _modulePageRepository;
        IModuleInRoleRepository _moduleInRoleRepository;
        IRoleRepository _roleRepository;
        IDepartmentRepository _departmentRepository;
        IEmploymentTypeRepository _employmentTypeRepository;
        IUserRoleRepository _userRoleRepository;
        //IDocumentTypeRepository _documentTypeRepository;
        IAuditLogRepository _auditLogRepository;
        IAnnouncementRepository _announcementRepository;
        IEmailRepository _emailRepository;
        IEmployeeRepository _employeeRepository;
        IHolidayRepository _holidayRepository;


        public MaintenanceController(IModulePageRepository modulePageRepository, IModuleInRoleRepository moduleInRoleRepository,
            IRoleRepository roleRepository, IUserRepository userRepository, IDepartmentRepository DepartmentRepository, IEmploymentTypeRepository employmentTypeRepository,
            IUserRoleRepository userRoleRepository, IAuditLogRepository auditLogRepository, IAnnouncementRepository announcementRepository,IEmailRepository emailRepository,
            IEmployeeRepository employeeRepository, IHolidayRepository holidayRepository)
        {
            this._userRepository = userRepository;
            this._moduleInRoleRepository = moduleInRoleRepository;
            this._modulePageRepository = modulePageRepository;
            this._roleRepository = roleRepository;
            this._departmentRepository = DepartmentRepository;
            this._employmentTypeRepository = employmentTypeRepository;
            this._userRoleRepository = userRoleRepository;
            //this._documentTypeRepository = documentTypeRepository;
            this._auditLogRepository = auditLogRepository;
            this._announcementRepository = announcementRepository;
            this._emailRepository = emailRepository;
            this._employeeRepository = employeeRepository;
            _holidayRepository = holidayRepository;
        }


        #region User
        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _userRepository.GetAllUsers());
        }


        [HttpPost]
        [Route("GetUserRoleListById")]
        public async Task<IActionResult> GetUserRoleListById(UserViewModel model)
        {
            //return Ok(await _userRepository.GetUserById(model.UserId));
            return Ok(await _userRepository.GetUserRoleListById(model.UserId));
        }

        [HttpPost]
        [Route("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UserViewModel model)
        {
            var userIdexist = await _userRepository.GetUserById(model.UserId);
            if (userIdexist == null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "User Id does not exist");
            }
            if (!await _roleRepository.IsExistsRole(model.RoleId))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Role Id does not exist");
            }

            var result = await _userRepository.UpdateUser(model);
            return StatusCode(result.statuscode, result.message);
        }

        [HttpPost]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromBody] UserViewModel model)
        {

            var result = await _userRepository.Delete(model.UserId);
            return StatusCode(result.statuscode, result.message);
        }

        #endregion

        #region Roles

        [HttpGet]
        [Route("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            return Ok(await _roleRepository.GetAllRoles());
        }

        [HttpPost]
        [Route("GetRoleById")]
        public async Task<IActionResult> GetRoleById(RoleViewModel model)
        {
            if (!await _roleRepository.IsExistsRole(model.RoleId))
            {
                return NotFound("Role Id invalid");
            }
            return Ok(await _roleRepository.GetRoleById(model.RoleId));
        }

        [HttpPost]
        [Route("SaveRole")]
        public async Task<IActionResult> SaveRole([FromBody] RoleViewModel model)
        {
            var result = await _roleRepository.Save(model);
            return StatusCode(result.statuscode, result.message);
        }
        [HttpPost]
        [Route("DeleteRole")]
        public async Task<IActionResult> DeleteRole([FromBody] RoleViewModel model)
        {
            var result = await _roleRepository.Delete(model);
            return StatusCode(result.statuscode, result.message);
        }
        #endregion

        #region Module Page

        [HttpPost]
        [Route("SaveModulePage")]
        public async Task<IActionResult> SaveModulePage([FromBody] ModulePageViewModel model)
        {
            var result = await _modulePageRepository.Save(model);
            return StatusCode(result.statuscode, result.message);
        }

        [HttpPost]
        [Route("GetModulePageById")]
        public async Task<IActionResult> GetModulePageById(ModulePageViewModel model)
        {
            if (!await _modulePageRepository.IsExistsModulePage(model.ModulePageId))
            {
                return NotFound("Module Page Id invalid");
            }
            return Ok(await _modulePageRepository.GetModulePageById(model.ModulePageId));
        }

        [HttpGet]
        [Route("GetAllModulePage")]
        public async Task<IActionResult> GetAllModulePage()
        {
            return Ok(await _modulePageRepository.GetAllModulePage());
        }

        [HttpPost]
        [Route("DeleteModulePage")]
        public async Task<IActionResult> DeleteModulePage([FromBody] ModulePageViewModel model)
        {
            var result = await _modulePageRepository.Delete(model);
            return StatusCode(result.statuscode, result.message);
        }

        #endregion

        #region Module In Role

        [HttpGet]
        [Route("GetAllModuleInRole")]
        public async Task<IActionResult> GetAllModuleInRole()
        {
            return Ok(await _moduleInRoleRepository.GetAllModuleInRole());
        }

        [HttpPost]
        [Route("GetModuleInRoleById")]
        public async Task<IActionResult> GetModuleInRoleById(ModuleInRoleViewModel model)
        {
            if (!await _moduleInRoleRepository.IsExistsModuleInRole(model.ModuleInRoleId))
            {
                return NotFound("Module In Role Id invalid");
            }
            return Ok(await _moduleInRoleRepository.GetModuleInRoleById(model.ModuleInRoleId));
        }

        [HttpPost]
        [Route("SaveModuleInRole")]
        public async Task<IActionResult> SaveModuleInRole([FromBody] ModuleInRoleViewModel model)
        {
            var result = await _moduleInRoleRepository.Save(model);
            return StatusCode(result.statuscode, result.message);
        }

        [HttpPost]
        [Route("DeleteModuleInRole")]
        public async Task<IActionResult> DeleteModuleInRole([FromBody] ModuleInRoleViewModel model)
        {
            var result = await _moduleInRoleRepository.Delete(model.ModuleInRoleId);
            return StatusCode(result.statuscode, result.message);
        }
        #endregion

        #region Department

        [HttpGet]
        [Route("GetAllDepartment")]
        public async Task<IActionResult> GetAllDepartment()
        {
            return Ok(await _departmentRepository.GetAllDepartment());
        }

        [HttpPost]
        [Route("GetDepartmentById")]
        public async Task<IActionResult> GetDepartmentById([FromBody] DepartmentViewModel model)
        {
            //if (!await _departmentRepository.IsExistsDepartment(model.DepartmentId))
            //{
            //	return NotFound("Department Id invalid");
            //}
            return Ok(await _departmentRepository.GetDepartmentById(model.DepartmentId));
        }

        [HttpPost]
        [Route("SaveDepartment")]
        public async Task<IActionResult> SaveDepartment([FromBody] DepartmentViewModel model)
        {
            var result = await _departmentRepository.Save(model);
            return StatusCode(result.statuscode, result.message);
        }
        [HttpPost]
        [Route("DeleteDepartment")]
        public async Task<IActionResult> DeleteDepartment([FromBody] DepartmentViewModel model)
        {
            var result = await _departmentRepository.Delete(model);
            return StatusCode(result.statuscode, result.message);
        }
        #endregion

        #region Employment Type

        [HttpGet]
        [Route("GetAllEmploymentType")]
        public async Task<IActionResult> GetAllEmploymentType()
        {
            return Ok(await _employmentTypeRepository.GetAllEmploymentType());
        }

        [HttpPost]
        [Route("GetEmploymentTypeById")]
        public async Task<IActionResult> GetEmploymentTypeById(EmploymentTypeViewModel model)
        {
            if (!await _employmentTypeRepository.IsExistsEmploymentType(model.EmploymentTypeId))
            {
                return NotFound("Employment Type Id invalid");
            }
            return Ok(await _employmentTypeRepository.GetEmploymentTypeById(model.EmploymentTypeId));
        }

        [HttpPost]
        [Route("SaveEmploymentType")]
        public async Task<IActionResult> SaveEmploymentType([FromBody] EmploymentTypeViewModel model)
        {
            var result = await _employmentTypeRepository.Save(model);
            return StatusCode(result.statuscode, result.message);
        }
        [HttpPost]
        [Route("DeleteEmploymentType")]
        public async Task<IActionResult> DeleteEmploymentType([FromBody] EmploymentTypeViewModel model)
        {
            var result = await _employmentTypeRepository.Delete(model);
            return StatusCode(result.statuscode, result.message);
        }
        #endregion

        #region Role Module

        [HttpPost]
        [Route("GetModuleAccessRoleByRoleId")]
        public async Task<IActionResult> GetModuleAccessRoleByRoleId(SystemManagementViewModel model)
        {
            return Ok(await _userRoleRepository.GetModuleAccessRoleByRoleId(model.RoleId));
        }

        [HttpPost]
        [Route("SaveRoleModule")]
        public async Task<IActionResult> SaveRoleModule([FromBody] RoleInModuleViewModel model)
        {
            var result = await _userRoleRepository.Save(model);
            return StatusCode(result.statuscode, result.message);
        }

        #endregion

        #region User Role

        [HttpGet]
        [Route("GetUserRole")]
        public async Task<IActionResult> GetUserRole()
        {
            return Ok(await _userRoleRepository.GetUserRole());
        }

        [HttpPost]
        [Route("DeleteUserRole")]
        public async Task<IActionResult> DeleteUserRole([FromBody] UserInRoleViewModel model)
        {
            var result = await _userRoleRepository.Delete(model);
            return StatusCode(result.statuscode, result.message);
        }
        #endregion



        #region Audit Logs
        [HttpPost]
        [Route("GetAuditLogById")]
        public async Task<IActionResult> GetAuditLogById([FromBody] AuditLogViewModel model)
        {
            return Ok(await _auditLogRepository.GetAuditLogById(model));
        }

        [HttpGet]
        [Route("GetAllAuditLogs")]
        public async Task<IActionResult> GetAllAuditLogs()
        {
            return Ok(await _auditLogRepository.GetAllAuditLogs());
        }
        #endregion



        #region Announcement

        [HttpGet]
        [Route("GetAllAnnouncement")]
        public async Task<IActionResult> GetAllAnnouncement()
        {
            return Ok(await _announcementRepository.GetAllAnnouncement());
        }

        [HttpPost]
        [Route("GetAnnouncementById")]
        public async Task<IActionResult> GetAnnouncementById([FromBody] AnnouncementViewModel model)
        {
            return Ok(await _announcementRepository.GetAnnouncementById(model.AnnouncementId));
        }

        [HttpPost]
        [Route("SaveAnnouncement")]
        public async Task<IActionResult> SaveAnnouncement([FromBody] AnnouncementViewModel model)
        {
            var result = await _announcementRepository.Save(model);
            return StatusCode(result.statuscode, result.message);
        }
        [HttpPost]
        [Route("DeleteAnnouncement")]
        public async Task<IActionResult> DeleteAnnouncement([FromBody] AnnouncementViewModel model)
        {
            var result = await _announcementRepository.Delete(model);
            return StatusCode(result.statuscode, result.message);
        }
        #endregion

        #region User Employee        

        [HttpGet]
        [Route("GetAllEmployee")]
        public async Task<IActionResult> GetAllEmployee()
        {
            return Ok(await _employeeRepository.GetAllEmployee());
        }

        [HttpPost]
        [Route("GetUserEmployeeRoleListById")]
        public async Task<IActionResult> GetUserEmployeeRoleListById(UserViewModel model)
        {

            return Ok(await _userRepository.GetUserEmployeeRoleListById(model.UserId));
        }

        [HttpPost]
        [Route("CreateUserAccount")]
        public async Task<IActionResult> CreateUserAccount([FromBody] UserViewModel model)
        {
            var userIdexist = await _userRepository.GetUserEmployeeByEmpId(model);

            if (model.UserId == 0 && userIdexist != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "User account already exist!");
            }

            else if (model.UserId == 0 && userIdexist ==null)
            {
                var result = await _userRepository.CreateUserAccount(model);

                if (result.UserId > 0)
                {
                    _emailRepository.SendSetPassword(result);
                    return StatusCode(StatusCodes.Status200OK, "Registration created successfully");
                }
                return StatusCode(StatusCodes.Status200OK, "Registration created successfully");
                // return StatusCode(result.statuscode, result.message);
            }           
            //update
            var updateUser = await _userRepository.UpdateUserAccount(model);
            return StatusCode(updateUser.statuscode, updateUser.message);

        }

        #endregion

        #region Holiday
        [HttpGet]
        [Route("GetAllHoliday")]
        public async Task<IActionResult> GetAllHoliday()
        {
            return Ok(await _holidayRepository.GetAllHoliday());
        }

        [HttpPost]
        [Route("GetHolidayById")]
        public async Task<IActionResult> GetHolidayById([FromBody] HolidayViewModel model)
        {         
            return Ok(await _holidayRepository.GetHolidayById(model.HolidayId));
        }

        [HttpPost]
        [Route("SaveHoliday")]
        public async Task<IActionResult> SaveHoliday([FromBody] HolidayViewModel model)
        {
            var result = await _holidayRepository.Save(model);
            return StatusCode(result.statuscode, result.message);
        }
        [HttpPost]
        [Route("DeleteHoliday")]
        public async Task<IActionResult> DeleteHoliday([FromBody] HolidayViewModel model)
        {
            var result = await _holidayRepository.Delete(model);
            return StatusCode(result.statuscode, result.message);
        }
        #endregion
    }
}
