using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using static QRCoder.PayloadGenerator;
using System.Runtime.Intrinsics.X86;

namespace DCI.Repositories
{
    public class EmployeeRepository : IEmployeeRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        public EmployeeRepository(DCIdbContext context)
        {
            this._dbContext = context;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<IList<Form201ViewModel>> GetAllEmployee()
        {
         return await (from emp in _dbContext.Employee
                          join dtl in _dbContext.EmployeeWorkDetails on emp.EmployeeId equals dtl.EmployeeId into dtlGroup
                          from dtl in dtlGroup.DefaultIfEmpty()

                          join dpt in _dbContext.Department on dtl.DepartmentId equals dpt.DepartmentId into dptGroup
                          from dpt in dptGroup.DefaultIfEmpty()

                          join post in _dbContext.Position on dtl.Position equals post.PositionId into postGroup
                          from post in postGroup.DefaultIfEmpty()

                          join empstat in _dbContext.EmployeeStatus on dtl.EmployeeStatusId equals empstat.EmployeeStatusId into empstatGroup
                          from empstat in empstatGroup.DefaultIfEmpty()   

                          where emp.IsActive == true
                   
                          orderby dtl.EmployeeStatusId descending, emp.Lastname  descending , emp.Firstname descending
                           select new Form201ViewModel
                              {
                                  EmployeeId = emp.EmployeeId,
                                  EmployeeNo = emp.EmployeeNo,
                                  Email = emp.Email,
                                  Firstname = emp.Firstname,
                                  Middlename = emp.Middlename,
                                  Lastname = emp.Lastname,
                                  Sex = emp.Sex,
                                  Prefix = emp.Prefix,
                                  Suffix = emp.Suffix,
                                  Nickname = emp.Nickname,
                                  DateBirth = emp.DateBirth,
                                  MobileNoPersonal = emp.MobileNoPersonal,
                                  LandlineNo = emp.LandlineNo,
                                  PresentAddress = emp.PresentAddress,
                                  PermanentAddress = emp.PermanentAddress,
                                  EmailPersonal = emp.EmailPersonal,
                                  DateCreated = emp.DateCreated,
                                  CreatedBy = emp.CreatedBy,
                                  DateModified = emp.DateModified,
                                  ModifiedBy = emp.ModifiedBy,
                                  IsActive = emp.IsActive,

                                  PositionName = post.PositionName,
                                  DepartmentName = dpt.DepartmentName,
                                  EmployeeStatusName = empstat.EmployeeStatusName,
                                  DateHired = dtl.DateHired,

                              }).ToListAsync();

        }

        public async Task<Form201ViewModel?> GetEmployeeById(int empId)
        {
            try
            {
                //var xamedsa = _dbContext.EmployeeWorkDetails.Where(x => x.EmployeeId == empId).FirstOrDefault();

               var result =  await (from emp in _dbContext.Employee
                                    join dtl in _dbContext.EmployeeWorkDetails on emp.EmployeeId equals dtl.EmployeeId into dtlGroup
                                    from dtl in dtlGroup.DefaultIfEmpty()

                                    join dpt in _dbContext.Department on dtl.DepartmentId equals dpt.DepartmentId into dptGroup
                                     from dpt in dptGroup.DefaultIfEmpty()

                                     join post in _dbContext.Position on dtl.Position equals post.PositionId into postGroup
                                     from post in postGroup.DefaultIfEmpty()

                                     join empstat in _dbContext.EmployeeStatus on dtl.EmployeeStatusId equals empstat.EmployeeStatusId into empstatGroup
                                     from empstat in empstatGroup.DefaultIfEmpty()

                                     where emp.EmployeeId == empId
                                            select new Form201ViewModel
                                            {
                                                EmployeeId = emp.EmployeeId,
                                                EmployeeNo = emp.EmployeeNo,
                                                Firstname = emp.Firstname,
                                                Middlename = emp.Middlename,
                                                Lastname = emp.Lastname,
                                                Sex = emp.Sex,
                                                Prefix = emp.Prefix,
                                                Suffix = emp.Suffix,
                                                Nickname = emp.Nickname,
                                                DateBirth = emp.DateBirth,
                                                MobileNoPersonal = emp.MobileNoPersonal,
                                                CivilStatus = emp.CivilStatus,
                                                LandlineNo = emp.LandlineNo,
                                                PresentAddress = emp.PresentAddress,
                                                PermanentAddress = emp.PermanentAddress,
                                                EmailPersonal = emp.EmailPersonal,
                                                ContactPerson = emp.ContactPerson,
                                                ContactPersonNo = emp.ContactPersonNo,
                                                Email = emp.Email,
                                                SSSNo = dtl.SSSNo,
                                                Tin = dtl.Tin,
                                                Pagibig = dtl.Pagibig,
                                                Philhealth = dtl.Philhealth,
                                                NationalId = dtl.NationalId,
                                                MobileNoOffice = dtl.MobileNoOffice,
                                                DepartmentId = dtl.DepartmentId,
                                                DepartmentName = dpt.Description,
                                                JobFunction = dtl.JobFunction,
                                                DateHired = dtl.DateHired,
                                                PositionId = post.PositionId,
                                                PositionName = post.Description,
                                                ResignedDate = dtl.ResignedDate,
                                                BandLevel = dtl.BandLevel,
                                                EmployeeStatusId = empstat.EmployeeStatusId,
                                                EmployeeStatusName = empstat.Description,
                                                DateCreated = emp.DateCreated,
                                                CreatedBy = emp.CreatedBy,
                                                DateModified = emp.DateModified,
                                                ModifiedBy = emp.ModifiedBy,
                                                IsActive = emp.IsActive
                                            }).FirstOrDefaultAsync() ?? new Form201ViewModel();


                result.EmployeeStatusList = _dbContext.EmployeeStatus.Where(x => x.IsActive).ToList();
                result.DepartmentList = _dbContext.Department.Where(x => x.IsActive).ToList();
                result.PositionList = _dbContext.Position.Where(x => x.IsActive).ToList();

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public async Task<(int statuscode, string message)> Save(Form201ViewModel model)
        {
            try
            {
                if (model.EmployeeId == 0)
                {
                    Employee emp = new Employee();
                    emp.EmployeeNo = model.EmployeeNo;
                    emp.Email = model.Email;
                    emp.Firstname = model.Firstname;
                    emp.Middlename = model.Middlename;
                    emp.Lastname = model.Lastname;
                    emp.Sex = model.Sex;
                    emp.Prefix = model.Prefix;
                    emp.Suffix = model.Suffix;
                    emp.Nickname = model.Nickname;
                    emp.DateBirth = model.DateBirth;
                    emp.CivilStatus = model.CivilStatus;
                    emp.MobileNoPersonal = model.MobileNoPersonal;
                    emp.LandlineNo = model.LandlineNo;
                    emp.PresentAddress = model.PresentAddress;
                    emp.PermanentAddress = model.PermanentAddress;
                    emp.EmailPersonal = model.EmailPersonal;
                    emp.ContactPerson = model.ContactPerson;
                    emp.ContactPersonNo = model.ContactPersonNo;
                    emp.DateCreated = DateTime.Now;
                    emp.CreatedBy = model.CreatedBy;
                    emp.DateModified = null;
                    emp.ModifiedBy = null;
                    emp.IsActive = true;
                    await _dbContext.Employee.AddAsync(emp);
                    await _dbContext.SaveChangesAsync();

                    EmployeeWorkDetails dtl = new EmployeeWorkDetails();
                    dtl.EmployeeId = emp.EmployeeId; 
                    dtl.SSSNo = model.SSSNo;
                    dtl.Tin = model.Tin;
                    dtl.Pagibig = model.Pagibig;
                    dtl.Philhealth = model.Philhealth;
                    dtl.NationalId = model.NationalId;
                    dtl.MobileNoOffice = model.MobileNoOffice;
                    dtl.EmployeeStatusId = model.EmployeeStatusId;
                    dtl.DepartmentId = model.DepartmentId;
                    dtl.JobFunction = model.JobFunction;
                    dtl.DateHired = model.DateHired;
                    dtl.Position = model.PositionId;
                    dtl.ResignedDate = model.ResignedDate;
                    dtl.BandLevel = model.BandLevel;
                    dtl.DateModified = null;
                    dtl.DateModified = null;
                    dtl.IsActive = true;
                    await _dbContext.EmployeeWorkDetails.AddAsync(dtl);
                    await _dbContext.SaveChangesAsync();
                    return (StatusCodes.Status200OK, "Registration successful");
                }
                else
                {

                    var emp = await _dbContext.Employee.FirstOrDefaultAsync(x => x.EmployeeId == model.EmployeeId);
                    emp.EmployeeNo = model.EmployeeNo;
                    emp.Email = model.Email;
                    emp.Firstname = model.Firstname;
                    emp.Middlename = model.Middlename;
                    emp.Lastname = model.Lastname;
                    emp.Sex = model.Sex;
                    emp.Prefix = model.Prefix;
                    emp.Suffix = model.Suffix;
                    emp.Nickname = model.Nickname;
                    emp.DateBirth = model.DateBirth;
                    emp.MobileNoPersonal = model.MobileNoPersonal;
                    emp.LandlineNo = model.LandlineNo;
                    emp.CivilStatus = model.CivilStatus;
                    emp.DateBirth = model.DateBirth;
                    emp.PresentAddress = model.PresentAddress;
                    emp.PermanentAddress = model.PermanentAddress;
                    emp.EmailPersonal = model.EmailPersonal;
                    emp.ContactPerson = model.ContactPerson;
                    emp.ContactPersonNo = model.ContactPersonNo;
                    //emp.DateCreated = emp.DateCreated;
                    //emp.CreatedBy = emp.CreatedBy;
                    emp.DateModified = DateTime.Now;
                    emp.ModifiedBy = model.ModifiedBy;
                    _dbContext.Employee.Entry(emp).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();


                    var dtl = await _dbContext.EmployeeWorkDetails.FirstOrDefaultAsync(x => x.EmployeeId == model.EmployeeId);
              
                    dtl.SSSNo = model.SSSNo;
                    dtl.Tin = model.Tin;
                    dtl.Pagibig = model.Pagibig;
                    dtl.Philhealth = model.Philhealth;
                    dtl.NationalId = model.NationalId;
                    dtl.MobileNoOffice = model.MobileNoOffice;
                    dtl.EmployeeStatusId = model.EmployeeStatusId;                   
                    dtl.DepartmentId = model.DepartmentId;
                    dtl.JobFunction = model.JobFunction;
                    dtl.DateHired = model.DateHired;
                    dtl.Position = model.PositionId;
                    dtl.ResignedDate = model.ResignedDate;
                    dtl.BandLevel = model.BandLevel;
                    dtl.DateModified = DateTime.Now;
                    dtl.ModifiedBy = model.ModifiedBy;
                    dtl.IsActive = true;
                    _dbContext.EmployeeWorkDetails.Entry(dtl).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    return (StatusCodes.Status200OK, "Registration updated");
                }
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


        public async Task<(int statuscode, string message)> Update201Form(Form201ViewModel model)
        {
            try
            {        

                    var emp = await _dbContext.Employee.FirstOrDefaultAsync(x => x.EmployeeId == model.EmployeeId);               
                    emp.Nickname = model.Nickname;    
                    emp.MobileNoPersonal = model.MobileNoPersonal;
                    emp.EmailPersonal = model.EmailPersonal;
                    emp.ContactPerson = model.ContactPerson;
                emp.ContactPersonNo = model.ContactPersonNo;
                    emp.DateModified = DateTime.Now;
                    emp.ModifiedBy = model.ModifiedBy;
                    _dbContext.Employee.Entry(emp).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();

                    return (StatusCodes.Status200OK, "Registration updated");
              
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

        public async Task<(int statuscode, string message)> Delete(Form201ViewModel model)
        {
            try
            {
                var entity = await _dbContext.Employee.FirstOrDefaultAsync(x => x.EmployeeId == model.EmployeeId && x.IsActive == true);
                if (entity == null)
                {
                    return (StatusCodes.Status406NotAcceptable, "Invalid Employee Id");
                }

                entity.IsActive = false;
                entity.ModifiedBy = model.ModifiedBy;
                entity.DateModified = DateTime.Now;
                _dbContext.Employee.Entry(entity).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return (StatusCodes.Status200OK, "Successfully deleted");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return (StatusCodes.Status400BadRequest, ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

    }
}
