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
                    if (entity.ModulePageId == 3)
                    {
                        SysManageVM.File201 = true;
                    }
                    else if (entity.ModulePageId == 4)
                    {
                        SysManageVM.Leave = true;
                    }
                    else if (entity.ModulePageId == 5)
                    {
                        SysManageVM.DailyTimeRecord = true;
                    }
                    else if (entity.ModulePageId == 6)
                    {
                        SysManageVM.DepartmentMain = true;
                    }
                    else if (entity.ModulePageId == 7)
                    {
                        SysManageVM.JobApplicants = true;
                    }
                    else if (entity.ModulePageId == 8)
                    {
                        SysManageVM.DTRManagement = true;
                    }
                    else if (entity.ModulePageId == 9)
                    {
                        SysManageVM.EmployeeManagement = true;
                    }
                    else if (entity.ModulePageId == 10)
                    {
                        SysManageVM.Administration = true;
                    }
                    else if (entity.ModulePageId == 11)
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
                    else if (entity.ModulePageId == 12)
                    {
                        SysManageVM.DepartmentSub = true;
                        if (entity.View == true)
                            SysManageVM.DepartmentSubView = true;
                        if (entity.Add == true)
                            SysManageVM.DepartmentSubAdd = true;
                        if (entity.Update == true)
                            SysManageVM.DepartmentSubUpdate = true;
                        if (entity.Delete == true)
                            SysManageVM.DepartmentSubDelete = true;
                        if (entity.Import == true)
                            SysManageVM.DepartmentSubImport = true;
                        if (entity.Export == true)
                            SysManageVM.DepartmentSubExport = true;
                    }
                    else if (entity.ModulePageId == 13)
                    {
                        SysManageVM.EmployeeType = true;
                        if (entity.View == true)
                            SysManageVM.EmployeeTypeView = true;
                        if (entity.Add == true)
                            SysManageVM.EmployeeTypeAdd = true;
                        if (entity.Update == true)
                            SysManageVM.EmployeeTypeUpdate = true;
                        if (entity.Delete == true)
                            SysManageVM.EmployeeTypeDelete = true;
                        if (entity.Import == true)
                            SysManageVM.EmployeeTypeImport = true;
                        if (entity.Export == true)
                            SysManageVM.EmployeeTypeExport = true;
                    }
                    else if (entity.ModulePageId == 14)
                    {
                        SysManageVM.Announcement = true;
                        if (entity.View == true)
                            SysManageVM.AnnouncementView = true;
                        if (entity.Add == true)
                            SysManageVM.AnnouncementAdd = true;
                        if (entity.Update == true)
                            SysManageVM.AnnouncementUpdate = true;
                        if (entity.Delete == true)
                            SysManageVM.AnnouncementDelete = true;
                        if (entity.Import == true)
                            SysManageVM.AnnouncementImport = true;
                        if (entity.Export == true)
                            SysManageVM.AnnouncementExport = true;
                    }
                    else if (entity.ModulePageId == 15)
                    {
                        SysManageVM.SystemManagement = true;
                        if (entity.View == true)
                            SysManageVM.SystemManagementView = true;
                        if (entity.Add == true)
                            SysManageVM.SystemManagementAdd = true;
                        if (entity.Update == true)
                            SysManageVM.SystemManagementUpdate = true;
                        if (entity.Delete == true)
                            SysManageVM.SystemManagementDelete = true;
                        if (entity.Import == true)
                            SysManageVM.SystemManagementImport = true;
                        if (entity.Export == true)
                            SysManageVM.SystemManagementExport = true;
                    }
                    else if (entity.ModulePageId == 16)
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
                    else if (entity.ModulePageId == 17)
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
                moduleinRoleVM.View = true;
                moduleinRoleVM.CreatedBy = model.RoleVM.CreatedBy;
               // moduleinRoleVM.ModifiedBy = model.RoleVM.ModifiedBy;

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

            return (StatusCodes.Status200OK, "Successfully saved");
        }

        public async Task<IList<UserInRoleViewModel>> GetUserRole()
        {
            var role_entity = await _dbContext.Role.Where(x => x.IsActive == true).ToListAsync();
            var result = role_entity.Select(x => new UserInRoleViewModel
            {
                RoleId = x.RoleId,
                RoleName = x.RoleName,
                UserCount = 0,
                ModuleCount = 0,
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

