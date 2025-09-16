using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using DCI.Core.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using System.Security.Cryptography.X509Certificates;
using System.Data;

namespace DCI.Repositories
{
    public class UserRoleRepository : IUserRoleRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        IModulePageRepository _modulePageRepository;
        IModuleInRoleRepository _moduleInRoleRepository;
        IRoleRepository _roleRepository;
        public UserRoleRepository(DCIdbContext context, IModulePageRepository modulePageRepository, IModuleInRoleRepository moduleInRoleRepository,
            IRoleRepository roleRepository)
        {
            this._dbContext = context;
            this._moduleInRoleRepository = moduleInRoleRepository;
            this._modulePageRepository = modulePageRepository;
            this._roleRepository = roleRepository;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }

     

        public async Task<SystemManagementViewModel> GetModuleAccessRoleByRoleId(int roleId)
        {
            try
            {
                
                SystemManagementViewModel SysManageVM = new SystemManagementViewModel();
                RoleInModuleViewModel vm = new RoleInModuleViewModel();
                List<ModuleInRoleViewModel> ModuleInRoleList = new List<ModuleInRoleViewModel>();


                var moduleInRoleEntity = await _dbContext.ModuleInRole.Where(x => x.RoleId == roleId).ToListAsync();
                var roleEntity = await _dbContext.Role.Where(x => x.RoleId == roleId).FirstOrDefaultAsync();

                foreach (var entity in moduleInRoleEntity)
                {
					if (entity.ModulePageId == (int)EnumModulePage.Dashboard)
					{
						SysManageVM.Dashboard = true;
					}
                    else if (entity.ModulePageId == (int)EnumModulePage.Form201)
                    {
                        SysManageVM.Form201 = true;
                        if (entity.View == true)
                            SysManageVM.Form201View = true;
                        if (entity.Add == true)
                            SysManageVM.Form201Add = true;
                        if (entity.Update == true)
                            SysManageVM.Form201Update = true;
                        if (entity.Delete == true)
                            SysManageVM.Form201Delete = true;
                        if (entity.Import == true)
                            SysManageVM.Form201Import = true;
                        if (entity.Export == true)
                            SysManageVM.Form201Export = true;
                    }
                    else if (entity.ModulePageId == (int)EnumModulePage.Administration)
                    {
                        SysManageVM.Administration = true;
                    }
                    else if (entity.ModulePageId == (int)EnumModulePage.UserManagement)
                    {
                        SysManageVM.UserManagement = true;
						if (entity.View == true)
							SysManageVM.UserManagementView = true;
						if (entity.Add == true)
							SysManageVM.UserManagementAdd = true;
						if (entity.Update == true)
							SysManageVM.UserManagementUpdate = true;
						if (entity.Delete == true)
							SysManageVM.UserManagementDelete = true;
						if (entity.Import == true)
							SysManageVM.UserManagementImport = true;
						if (entity.Export == true)
							SysManageVM.UserManagementExport = true;
					}
                    else if (entity.ModulePageId == (int)EnumModulePage.Department)
                    {
                        SysManageVM.Department = true;
                        if (entity.View == true)
                            SysManageVM.DepartmentView = true;
                        if (entity.Add == true)
                            SysManageVM.DepartmentAdd = true;
                        if (entity.Update == true)
                            SysManageVM.DepartmentUpdate = true;
                        if (entity.Delete == true)
                            SysManageVM.DepartmentDelete = true;
                        if (entity.Import == true)
                            SysManageVM.DepartmentImport = true;
                        if (entity.Export == true)
                            SysManageVM.DepartmentExport = true;
                    }
					else if (entity.ModulePageId == (int)EnumModulePage.EmployeeMaster)
                    {
                        SysManageVM.EmployeeMaster = true;
						if (entity.View == true)
							SysManageVM.EmployeeMasterView = true;
						if (entity.Add == true)
							SysManageVM.EmployeeMasterAdd = true;
						if (entity.Update == true)
							SysManageVM.EmployeeMasterUpdate = true;
						if (entity.Delete == true)
							SysManageVM.EmployeeMasterDelete = true;
						if (entity.Import == true)
							SysManageVM.EmployeeMasterImport = true;
						if (entity.Export == true)
							SysManageVM.EmployeeMasterExport = true;
					}
                    else if (entity.ModulePageId == (int)EnumModulePage.Holiday)
                    {
                        SysManageVM.Holiday = true;
						if (entity.View == true)
							SysManageVM.HolidayView = true;
						if (entity.Add == true)
							SysManageVM.HolidayAdd = true;
						if (entity.Update == true)
							SysManageVM.HolidayUpdate = true;
						if (entity.Delete == true)
							SysManageVM.HolidayDelete = true;
						if (entity.Import == true)
							SysManageVM.HolidayImport = true;
						if (entity.Export == true)
							SysManageVM.HolidayExport = true;
					}
                    else if (entity.ModulePageId == (int)EnumModulePage.UserRole)
                    {
                        SysManageVM.UserRoleManagement = true;
						if (entity.View == true)
							SysManageVM.UserRoleManagementView = true;
						if (entity.Add == true)
							SysManageVM.UserRoleManagementAdd = true;
						if (entity.Update == true)
							SysManageVM.UserRoleManagementUpdate = true;
						if (entity.Delete == true)
							SysManageVM.UserRoleManagementDelete = true;
						if (entity.Import == true)
							SysManageVM.UserRoleManagementImport = true;
						if (entity.Export == true)
							SysManageVM.UserRoleManagementExport = true;
					}
                    else if (entity.ModulePageId == (int)EnumModulePage.SystemManagement)
                    {
                        SysManageVM.SystemManagement = true;
                    }
                    else if (entity.ModulePageId == (int)EnumModulePage.AuditTrail)
                    {
                        SysManageVM.AuditTrail = true;
						if (entity.View == true)
							SysManageVM.AuditTrailView = true;
						if (entity.Add == true)
							SysManageVM.AuditTrailAdd = true;
						if (entity.Update == true)
							SysManageVM.AuditTrailUpdate = true;
						if (entity.Delete == true)
							SysManageVM.AuditTrailDelete = true;
						if (entity.Import == true)
							SysManageVM.AuditTrailImport = true;
						if (entity.Export == true)
							SysManageVM.AuditTrailExport = true;
					}
                    else if (entity.ModulePageId == (int)EnumModulePage.Todo)
                    {
                        SysManageVM.Todo = true;
                       
                    }
                    else if (entity.ModulePageId == (int)EnumModulePage.Reports)
                    {
                        SysManageVM.Reports = true;
                    }
                    else if (entity.ModulePageId == (int)EnumModulePage.DTRManagement)
                    {
                        SysManageVM.DTRManagement = true;
                    }
                    else if (entity.ModulePageId == (int)EnumModulePage.Attendance)
                    {
                        SysManageVM.Attendance = true;
                    }
                    else if (entity.ModulePageId == (int)EnumModulePage.DailyTimeRecord)
                    {
                        SysManageVM.DailyTimeRecord = true;
                    }
                    else if (entity.ModulePageId == (int)EnumModulePage.Leave)
                    {
                        SysManageVM.Leave = true;
                    }
                    else if (entity.ModulePageId == (int)EnumModulePage.Overtime)
                    {
                        SysManageVM.Overtime = true;
                    }
                    else if (entity.ModulePageId == (int)EnumModulePage.Position)
                    {                      
                        SysManageVM.Position = true;
                        if (entity.View == true)
                            SysManageVM.PositionView = true;
                        if (entity.Add == true)
                            SysManageVM.PositionAdd = true;
                        if (entity.Update == true)
                            SysManageVM.PositionUpdate = true;
                        if (entity.Delete == true)
                            SysManageVM.PositionDelete = true;
                        if (entity.Import == true)
                            SysManageVM.PositionImport = true;
                        if (entity.Export == true)
                            SysManageVM.PositionExport = true;
                    }
                    else if (entity.ModulePageId == (int)EnumModulePage.WFH)
                    {
                        SysManageVM.WFH = true;
                    }
                    else if (entity.ModulePageId == (int)EnumModulePage.Undertime)
                    {
                        SysManageVM.Undertime = true;
                    }
                    else if (entity.ModulePageId == (int)EnumModulePage.DTRCorrection)
                    {
                        SysManageVM.DTRCorrection = true;
                    }

                }
                SysManageVM.RoleId = roleEntity.RoleId;
                SysManageVM.RoleName = roleEntity.RoleName;
                SysManageVM.Description = roleEntity.Description;
                //vm.RoleVM = new RoleViewModel
                //{
                //    RoleId = roleEntity.RoleId,
                //    RoleName = roleEntity.RoleName,
                //    Description = roleEntity.Description
                //};

                //vm.ModuleInRoleList = ModuleInRoleList;
                ////vm.RoleVM = vm.RoleVM;
                //vm.Modules = null;
                return SysManageVM;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
            return null;
        }

        public async Task<(int statuscode, string message)> Save(RoleInModuleViewModel model)
        {
            // New ROLE
            

            RoleViewModel rolevm = new RoleViewModel();
            rolevm.RoleId = model.RoleVM.RoleId;
            rolevm.RoleName = model.RoleVM.RoleName;
            rolevm.Description = model.RoleVM.Description;
            rolevm.CreatedBy = model.RoleVM.CreatedBy;
            rolevm.DateCreated = model.RoleVM.DateCreated;
            rolevm.ModifiedBy = model.RoleVM.ModifiedBy;
            rolevm.DateModified = model.RoleVM.DateModified;
            var entity_role = _roleRepository.Save(rolevm);
            await Task.Delay(1000);

            ModuleInRoleViewModel moduleinRoleVM = new ModuleInRoleViewModel();


            var mainModuleValue = 0;
            var subModulesValue = 0;
            int _roleId = entity_role?.Result.entity != null ? entity_role.Result.entity.RoleId : 0;

            await _moduleInRoleRepository.DeletebyRoleId(_roleId);



            foreach (var module in model.Modules)
            {
                moduleinRoleVM.ModulePageId = module.Value?.MainModule != null ? Int32.Parse(module.Value.MainModule) : 0;
                moduleinRoleVM.RoleId = _roleId;
       
                moduleinRoleVM.CreatedBy = model.RoleVM.CreatedBy;
				moduleinRoleVM.View = false;
				moduleinRoleVM.Add = false;
				moduleinRoleVM.Update = false;
				moduleinRoleVM.Delete = false;
				moduleinRoleVM.Import = false;
				moduleinRoleVM.Export = false;

				if (module.Value?.SubModules?.Any() == true)
                {

                    foreach (var sub in module.Value.SubModules)
                    {
                        subModulesValue = sub != null ? Int32.Parse(sub) : 0;

                        if (subModulesValue == (int)EnumPermissionRole.View)
                        {
                            moduleinRoleVM.View = true;
                        }
                        else if (subModulesValue == (int)EnumPermissionRole.Add)
                        {
                            moduleinRoleVM.Add = true;
                        }
                        else if (subModulesValue == (int)EnumPermissionRole.Update)
                        {
                            moduleinRoleVM.Update = true;
                        }
                        else if (subModulesValue == (int)EnumPermissionRole.Delete)
                        {
                            moduleinRoleVM.Delete = true;
                        }
                        else if (subModulesValue == (int)EnumPermissionRole.Import)
                        {
                            moduleinRoleVM.Import = true;
                        }
                        else if (subModulesValue == (int)EnumPermissionRole.Export)
                        {
                            moduleinRoleVM.Export = true;
                        }
                    }
                }
                await _moduleInRoleRepository.Save(moduleinRoleVM);
            }
			//		return (StatusCodes.Status200OK, String.Format("Document {0} has been created successfully.", entity.DocNo));
			//				return (StatusCodes.Status200OK, String.Format("Document {0} has been updated successfully.", entity.DocNo));
   //         if(model.RoleVM.RoleId ==0)
   //         {
   //             message_box = String.Format("User Role {0} has been created successfully.", rolevm.RoleName);
			//}
   //         else
   //         {
			//	message_box = String.Format("User Role {0} has been updated successfully.", rolevm.RoleName);
			//}
			string message_box = model.RoleVM.RoleId == 0 ? String.Format("{0} has been created successfully.", rolevm.RoleName) : String.Format("{0} has been updated successfully.", rolevm.RoleName);
			return (StatusCodes.Status200OK, message_box);
        }

        public async Task<IList<UserInRoleViewModel>> GetUserRole()
        {
            var role_entity = await _dbContext.Role.Where(x => x.IsActive == true).ToListAsync();
            var user_entity = await _dbContext.User.Where(x => x.IsActive == true).ToListAsync();
            var moduleInRole_entity = await _dbContext.ModuleInRole.Where(x => x.IsActive == true && x.ModulePageId != (int)EnumModulePage.Administration).ToListAsync();

			var result = role_entity.Select(role => new UserInRoleViewModel
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
				UserCount = user_entity.Count(user => user.RoleId == role.RoleId),
				ModuleCount = moduleInRole_entity.Count(module => module.RoleId == role.RoleId),
                SubModuleCount = 0
            }).ToList();

            return result;
        }

        public async Task<(int statuscode, string message)> Delete(UserInRoleViewModel model)
        {
            try
            {
                var entity = await _dbContext.Role.FirstOrDefaultAsync(x => x.RoleId == model.RoleId);
                if (entity == null)
                {
                    return (StatusCodes.Status406NotAcceptable, "Invalid role Id");
                }

                entity.IsActive = false;
                entity.ModifiedBy = model.ModifiedBy;
                entity.DateModified = DateTime.Now;
                _dbContext.Role.Entry(entity).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return (StatusCodes.Status200OK, "Successfully deleted");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return (StatusCodes.Status406NotAcceptable, ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}

