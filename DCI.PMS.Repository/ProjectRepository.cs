using Dapper;
using DCI.Core.Common;
using DCI.Core.Helpers;
using DCI.Data;
using DCI.Models.Configuration;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.PMS.Models.Entities;
using DCI.PMS.Models.ViewModel;
using DCI.PMS.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Net.Sockets;

namespace DCI.PMS.Repository
{
    public class ProjectRepository : IProjectRepository, IDisposable
    {
        private readonly IOptions<DCI.Models.ViewModel.FileModel> _fileconfig;
        private readonly DCIdbContext _dbContext;
        private readonly PMSdbContext _pmsdbContext;
        private readonly IDbConnection _connection;
        public ProjectRepository(DCIdbContext context, PMSdbContext pmsContext, IOptions<DCI.Models.ViewModel.FileModel> fileconfig, IConfiguration configuration)
        {
            this._dbContext = context;
            this._pmsdbContext = pmsContext;
            this._fileconfig = fileconfig;
            // this._connection = connection;

            var connStr = configuration.GetConnectionString("DCIConnectionPMS") ?? throw new InvalidOperationException("Connection string not found.");
            _connection = new SqlConnection(connStr);


        }
        public void Dispose()
        {
            _dbContext.Dispose();
            _pmsdbContext.Dispose();
        }

        public async Task<ProjectViewModel> GetProjectId(int projectId)
        {
            var users = _dbContext.User.AsNoTracking().ToList();
            var projects = _pmsdbContext.Project.AsNoTracking().ToList();

            var query = projects
                 .Join(
                     users,
                     p => p.CreatedBy,
                     u => u.UserId,
                     (p, u) => new ProjectViewModel
                     {
                         ProjectNo = p.ProjectNo,
                         IsActive = p.IsActive
                     }
                 );
            var result = query.FirstOrDefault();
            if (result == null)
            {
                result = new ProjectViewModel();
            }

            return result;
        }

        public async Task<IList<ProjectViewModel>> GetAllProject()
        {

            try
            {
                var users = await _dbContext.User
                                        .AsNoTracking()
                                        .Where(p => p.IsActive)
                                        .Select(u => new
                                        {
                                            u.UserId,
                                            u.Fullname
                                        })
                                        .ToListAsync();

                var projects = await _pmsdbContext.Project
                                        .AsNoTracking()
                                        .Where(p => p.IsActive)
                                        .Select(p => new
                                        {
                                            p.ProjectCreationId,
                                            p.ProjectNo,
                                            p.ProjectName,
                                            p.ClientId,
                                            p.NOADate,
                                            p.NTPDate,
                                            p.MOADate,
                                            p.ProjectDuration,
                                            p.ProjectCost,
                                            p.ModeOfPayment,
                                            p.Status,
                                            p.CreatedBy,
                                            p.IsActive
                                        })
                                        .ToListAsync();

                var status = await _pmsdbContext.Status
                                       .AsNoTracking()
                                       .Where(p => p.IsActive)
                                       .Select(s => new
                                       {
                                           s.StatusId,
                                           s.StatusName
                                       })
                                       .ToListAsync();

                var _clientList = await _pmsdbContext.Client
                      .AsNoTracking()
                      .Where(c => c.IsActive)
                      .Select(c => new ClientViewModel
                      {
                          ClientId = c.ClientId,
                          ClientName = c.ClientName,
                          Description = c.Description,
                          DateCreated = c.DateCreated,
                          CreatedBy = c.CreatedBy,
                          IsActive = c.IsActive
                      })
                      .ToListAsync();


                var result = from p in projects
                             join u in users on p.CreatedBy equals u.UserId
                             join s in status on p.Status equals s.StatusId
                             join c in _clientList on p.ClientId equals c.ClientId
                             select new ProjectViewModel
                             {
                                 ProjectCreationId = p.ProjectCreationId,
                                 ProjectNo = p.ProjectNo,
                                 ProjectName = p.ProjectName,
                                 NOADate = p.NOADate,
                                 NTPDate = p.NTPDate,
                                 MOADate = p.MOADate,
                                 ProjectDuration = p.ProjectDuration,
                                 ProjectCost = p.ProjectCost,
                                 ModeOfPayment = p.ModeOfPayment,
                                 ClientId = p.ClientId,
                                 ClientName = c.ClientName,
                                 IsActive = p.IsActive,
                                 CreatedName = u.Fullname,
                                 StatusName = s.StatusName
                             };

                return result.ToList();
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

        public async Task<ProjectViewModel> GetProjectById(ProjectViewModel model)
        {

            try
            {
                var users = await _dbContext.User
                                        .AsNoTracking()
                                        .Where(p => p.IsActive)
                                        .Select(u => new
                                        {
                                            u.UserId,
                                            u.Fullname
                                        })
                                        .ToListAsync();

                var _clientList = await _pmsdbContext.Client
                                    .AsNoTracking()
                                    .Where(c => c.IsActive)
                                    .Select(c => new ClientViewModel
                                    {
                                        ClientId = c.ClientId,
                                        ClientName = c.ClientName,
                                        Description = c.Description,
                                        DateCreated = c.DateCreated,
                                        CreatedBy = c.CreatedBy,
                                        IsActive = c.IsActive
                                    })
                                    .ToListAsync();

                var status = await _pmsdbContext.Status
                                      .AsNoTracking()
                                      .Where(p => p.IsActive)
                                      .Select(s => new
                                      {
                                          s.StatusId,
                                          s.StatusName
                                      })
                                      .ToListAsync();


                var projects = await _pmsdbContext.Project
                                        .AsNoTracking()
                                        .Where(p => p.IsActive)
                                        .Select(p => new
                                        {
                                            p.ProjectCreationId,
                                            p.ProjectNo,
                                            p.ProjectName,
                                            p.ClientId,
                                            p.NOADate,
                                            p.NTPDate,
                                            p.MOADate,
                                            p.ProjectDuration,
                                            p.ProjectCost,
                                            p.ModeOfPayment,
                                            p.Status,
                                            p.CreatedBy,
                                            p.IsActive
                                        })
                                        .ToListAsync();

                var result = (from p in projects
                              join u in users on p.CreatedBy equals u.UserId
                              join c in _pmsdbContext.Client on p.ClientId equals c.ClientId
                              join s in status on p.Status equals s.StatusId
                              where p.ProjectCreationId == model.ProjectCreationId
                              select new ProjectViewModel
                              {
                                  ProjectCreationId = p.ProjectCreationId,
                                  ProjectNo = p.ProjectNo,
                                  ProjectName = p.ProjectName,
                                  ClientId = p.ClientId,
                                  ClientName = c.ClientName,
                                  NOADate = p.NOADate,
                                  NTPDate = p.NTPDate,
                                  MOADate = p.MOADate,
                                  ProjectDuration = p.ProjectDuration,
                                  ProjectCost = p.ProjectCost,
                                  ModeOfPayment = p.ModeOfPayment,
                                  IsActive = p.IsActive,
                                  CreatedName = u.Fullname,
                                  StatusName = s.StatusName,
                                  ModeOfPaymentName = p.ModeOfPayment == 1 ? "Milestone Based" : p.ModeOfPayment == 2 ? "Transacational Based" : "Full Payment"
                              }).FirstOrDefault();


                var attachList = _pmsdbContext.Attachment
                                .AsNoTracking()
                                .Where(a => a.IsActive && a.ProjectCreationId == model.ProjectCreationId)
                                .AsEnumerable()
                                .Join(
                                    users,
                                    a => a.CreatedBy,
                                    u => u.UserId,
                                    (a, u) => new AttachmentViewModel
                                    {
                                        AttachmentId = a.AttachmentId,
                                        ProjectCreationId = a.ProjectCreationId,
                                        MileStoneId = a.MileStoneId,
                                        DeliverableId = a.DeliverableId,
                                        AttachmentType = a.AttachmentType,
                                        Filename = a.Filename,
                                        FileLocation = a.FileLocation,
                                        CreatedBy = a.CreatedBy,
                                        CreatedName = u.Fullname,
                                        DateCreated = a.DateCreated,
                                        IsActive = a.IsActive
                                    }
                                )
                                .OrderByDescending(x => x.AttachmentId)
                                .ToList();

                var statusLookup = status.ToDictionary(x => x.StatusId, x => x.StatusName);

                var milestone = _pmsdbContext.Milestone
                                .AsNoTracking()
                                .Where(a => a.IsActive && a.ProjectCreationId == model.ProjectCreationId)
                                .AsEnumerable()
                                .Join(
                                    users,
                                    a => a.CreatedBy,
                                    u => u.UserId,
                                    (a, u) => new MilestoneViewModel
                                    {
                                        MileStoneId = a.MileStoneId,
                                        ProjectCreationId = a.ProjectCreationId,
                                        MilestoneName = a.MilestoneName,
                                        Percentage = a.Percentage,
                                        TargetCompletedDate = a.TargetCompletedDate,
                                        ActualCompletionDate = a.ActualCompletionDate,
                                        PaymentStatus = a.PaymentStatus,
                                        Status = a.Status,
                                        StatusName = statusLookup.TryGetValue(a.Status, out var name) ? name : "",
                                        Remarks = a.Remarks,
                                        CreatedBy = a.CreatedBy,
                                        CreatedName = u.Fullname,
                                        DateCreated = a.DateCreated,
                                        IsActive = a.IsActive,
                                        DeliverableList = _pmsdbContext.Deliverable
                                               .AsNoTracking()
                                               .Where(a => a.IsActive && a.MileStoneId == model.ProjectCreationId)
                                               .AsEnumerable()
                                               .Join(
                                                   users,
                                                   a => a.CreatedBy,
                                                   u => u.UserId,
                                                   (a, u) => new DeliverableViewModel
                                                   {
                                                       DeliverableId = a.DeliverableId,
                                                       MileStoneId = a.MileStoneId,
                                                       DeliverableName = a.DeliverableName,
                                                       Status = a.Status,
                                                       CreatedBy = a.CreatedBy,
                                                       CreatedName = u.Fullname,
                                                       DateCreated = a.DateCreated,
                                                       IsActive = a.IsActive,
                                                       StatusName = statusLookup.TryGetValue(a.Status, out var name) ? name : "",
                                                   }
                                               )
                                                .ToList()
                                    }
                                )
                                 .ToList();

                //var deliverables = _pmsdbContext.Deliverable
                //   .AsNoTracking()
                //   .Where(a => a.IsActive && a.MileStoneId == model.ProjectCreationId)
                //   .AsEnumerable()
                //   .Join(
                //       users,
                //       a => a.CreatedBy,
                //       u => u.UserId,
                //       (a, u) => new DeliverableViewModel
                //       {
                //           DeliverableId = a.DeliverableId,
                //           MileStoneId = a.MileStoneId,
                //           ProjectCreationId = a.ProjectCreationId,
                //          // StatusName = a.MilesttaoneName,

                //           Status = a.Status,

                //           CreatedBy = a.CreatedBy,
                //           CreatedName = u.Fullname,
                //           DateCreated = a.DateCreated,
                //           IsActive = a.IsActive
                //       }
                //   )
                //    .ToList();

                if (result == null)
                {
                    result = new ProjectViewModel();
                }

                result.ClientList = _clientList;
                result.AttachmentList = attachList;//.Where(x => x.MileStoneId == 0 && x.DeliverableId == 0 && x.AttachmentType == (int)EnumAttachmentType.OTHER).ToList();

                var noa = attachList.FirstOrDefault(x => x.AttachmentType == (int)EnumAttachmentType.NOA && x.MileStoneId == 0 && x.DeliverableId == 0 && x.IsActive);
                result.IsNOAFile = noa is not null;
                result.NOAFileId = noa?.AttachmentId ?? 0;

                var ntp = attachList.FirstOrDefault(x => x.AttachmentType == (int)EnumAttachmentType.NTP && x.MileStoneId == 0 && x.DeliverableId == 0 && x.IsActive);
                result.IsNTPFile = ntp is not null;
                result.NTPFileId = ntp?.AttachmentId ?? 0;

                var moa = attachList.FirstOrDefault(x => x.AttachmentType == (int)EnumAttachmentType.MOA && x.MileStoneId == 0 && x.DeliverableId == 0 && x.IsActive);
                result.IsMOAFile = moa is not null;
                result.MOAFileId = moa?.AttachmentId ?? 0;

                result.MilestoneList = milestone;
                // result.DeliveryList = Deliverable;

                return result;

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


        public async Task<bool> IsExistsProject(int projectId)
        {
            return await _pmsdbContext.Project.AnyAsync(x => x.ProjectCreationId == projectId && x.IsActive == true);
        }

        public async Task SaveProject(ProjectViewModel model)
        {
            try
            {
                if (model.ProjectCreationId == 0)
                {
                    //Project entity = new Project();
                    //entity.ProjectCreationId = model.ProjectCreationId;
                    //entity.ClientId = model.ClientId;
                    //entity.ProjectNo =  await GenereteRequestNo();
                    //entity.ProjectName = model.ProjectName;
                    //entity.NOADate = model.NOADate;
                    //entity.NTPDate = model.NTPDate;
                    //entity.MOADate = model.MOADate;
                    //entity.ProjectDuration = model.ProjectDuration;
                    //entity.ProjectCost = model.ProjectCost;
                    //entity.ModeOfPayment = model.ModeOfPayment;
                    //entity.CreatedBy = model.CreatedBy;
                    //entity.DateCreated = DateTime.Now;
                    //entity.ModifiedBy = null;
                    //entity.DateModified = null;
                    //entity.IsActive = true;
                    //await _pmsdbContext.Project.AddAsync(entity);
                    //await _pmsdbContext.SaveChangesAsync();
                    await _pmsdbContext.Database.ExecuteSqlInterpolatedAsync($@"
                                INSERT INTO dbo.Project
                                (
                                    ProjectCreationId,
                                    ClientId,
                                    ProjectNo,
                                    ProjectName,
                                    NOADate,
                                    NTPDate,
                                    MOADate,
                                    ProjectDuration,
                                    ProjectCost,
                                    ModeOfPayment,
                                    CreatedBy,
                                    DateCreated,
                                    IsActive
                                )
                                VALUES
                                (
                                    {model.ProjectCreationId},
                                    {model.ClientId},
                                    {model.ProjectNo},
                                    {model.ProjectName},
                                    {model.NOADate},
                                    {model.NTPDate},
                                    {model.MOADate},
                                    {model.ProjectDuration},
                                    {model.ProjectCost},
                                    {model.ModeOfPayment},
                                    {model.CreatedBy},
                                    {DateTime.Now},
                                    1
                                )
                            ");

                    await SaveFile(model);
                    // return (StatusCodes.Status200OK, "Successfully saved");
                }
                else
                {
                    //var entity = await _pmsdbContext.Project.FirstOrDefaultAsync(x => x.ProjectCreationId == model.ProjectCreationId);
                    //entity.ProjectCreationId = model.ProjectCreationId;
                    //entity.ClientId = model.ClientId;
                    //entity.ProjectNo = model.ProjectNo;
                    //entity.ProjectName = model.ProjectName;
                    //entity.NOADate = model.NOADate;
                    //entity.NTPDate = model.NTPDate;
                    //entity.MOADate = model.MOADate;
                    //entity.ProjectDuration = model.ProjectDuration;
                    //entity.ProjectCost = model.ProjectCost;
                    //entity.ModeOfPayment = model.ModeOfPayment;
                    //entity.DateCreated = entity.DateCreated;
                    //entity.CreatedBy = entity.CreatedBy;
                    //entity.DateModified = DateTime.Now;
                    //entity.ModifiedBy = model.ModifiedBy;
                    //entity.IsActive = true;
                    //_pmsdbContext.Project.Entry(entity).State = EntityState.Modified;
                    //await _pmsdbContext.SaveChangesAsync();

                    await _pmsdbContext.Database.ExecuteSqlInterpolatedAsync($@"
                        UPDATE dbo.Project
                        SET
                            ClientId        = {model.ClientId},
                            ProjectName     = {model.ProjectName},
                            NOADate         = {model.NOADate},
                            NTPDate         = {model.NTPDate},
                            MOADate         = {model.MOADate},
                            ProjectDuration= {model.ProjectDuration},
                            ProjectCost     = {model.ProjectCost},
                            ModeOfPayment  = {model.ModeOfPayment},
                            ModifiedBy     = {model.ModifiedBy},
                            DateModified   = {DateTime.Now}
                        WHERE ProjectCreationId = {model.ProjectCreationId}
                    ");

                    await SaveFile(model);
                    //return (StatusCodes.Status200OK, "Successfully updated");
                }


            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                //return (StatusCodes.Status406NotAcceptable, ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private async Task SaveFile(ProjectViewModel model)
        {
            try
            {
                string fileloc = @"C:\\DCI App\\PMS\\" + model.ProjectCreationId.ToString() + @"\";

                if (!Directory.Exists(fileloc))
                    Directory.CreateDirectory(fileloc);


                if (model.NOAFile != null && model.NOAFile.Length > 0)
                {
                    string noa_filename = model.NOAFile.FileName;//Constants.Attachment_Type_NOA + DateTime.Now.ToString("yyyyMMddHHmmss") + Core.Common.Constants.Filetype_Pdf; //model.NOAFile.FileName;
                    string noa_filenameLocation = Path.Combine(fileloc, noa_filename);
                    using (var stream = new FileStream(noa_filenameLocation, FileMode.Create, FileAccess.Write))
                    {
                        model.NOAFile.CopyTo(stream);
                    }

                    await SaveAttachment(new AttachmentViewModel
                    {
                        ProjectCreationId = model.ProjectCreationId,
                        MileStoneId = 0,
                        DeliverableId = 0,
                        AttachmentType = (int)EnumAttachmentType.NOA,
                        Filename = noa_filename,
                        FileLocation = noa_filenameLocation,
                        CreatedBy = model.CreatedBy
                    });
                }

                if (model.NTPFile != null && model.NTPFile.Length > 0)
                {
                    string ntp_filename = Constants.Attachment_Type_NTP + DateTime.Now.ToString("yyyyMMddHHmmss") + Core.Common.Constants.Filetype_Pdf;// model.NTPFile.FileName;
                    string ntp_filenameLocation = Path.Combine(fileloc, ntp_filename);

                    using (var stream = new FileStream(ntp_filenameLocation, FileMode.Create, FileAccess.Write))
                    {
                        model.NTPFile.CopyTo(stream);
                    }

                    await SaveAttachment(new AttachmentViewModel
                    {
                        ProjectCreationId = model.ProjectCreationId,
                        MileStoneId = 0,
                        DeliverableId = 0,
                        AttachmentType = (int)EnumAttachmentType.NTP,
                        Filename = ntp_filename,
                        FileLocation = ntp_filenameLocation,
                        CreatedBy = model.CreatedBy
                    });

                }

                if (model.MOAFile != null && model.MOAFile.Length > 0)
                {
                    string moa_filename = Constants.Attachment_Type_MOA + DateTime.Now.ToString("yyyyMMddHHmmss") + Core.Common.Constants.Filetype_Pdf;// model.MOAFile.FileName;
                    string moa_filenameLocation = Path.Combine(fileloc, moa_filename);

                    using (var stream = new FileStream(moa_filenameLocation, FileMode.Create, FileAccess.Write))
                    {
                        model.MOAFile.CopyTo(stream);
                    }

                    await SaveAttachment(new AttachmentViewModel
                    {
                        ProjectCreationId = model.ProjectCreationId,
                        MileStoneId = 0,
                        DeliverableId = 0,
                        AttachmentType = (int)EnumAttachmentType.MOA,
                        Filename = moa_filename,
                        FileLocation = moa_filenameLocation,
                        CreatedBy = model.CreatedBy
                    });
                }


                if (model.OtherAttachment != null && model.OtherAttachment.Any())
                {
                    foreach (var file in model.OtherAttachment)
                    {
                        string _fileloc = fileloc + file.FileName;

                        await SaveAttachment(new AttachmentViewModel
                        {
                            ProjectCreationId = model.ProjectCreationId,
                            MileStoneId = 0,
                            DeliverableId = 0,
                            AttachmentType = (int)EnumAttachmentType.OTHER,
                            Filename = file.FileName,
                            FileLocation = _fileloc,
                            CreatedBy = model.CreatedBy
                        });
                    }
                }


            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public async Task<ProjectViewModel> GetMilestoneByProjectId(ProjectViewModel model)
        {

            try
            {
                var users = await _dbContext.User
                                        .AsNoTracking()
                                        .Where(p => p.IsActive)
                                        .Select(u => new
                                        {
                                            u.UserId,
                                            u.Fullname
                                        })
                                        .ToListAsync();

                var statusList = await _dbContext.Status
                             .AsNoTracking()
                             .Where(p => p.IsActive)
                             .Select(u => new StatusViewModel
                             {
                                 StatusId = u.StatusId,
                                 StatusName = u.StatusName
                             })
                             .ToListAsync();

                var milestone = _pmsdbContext.Milestone
                                        .AsNoTracking()
                                        .Where(p => p.IsActive).ToList();

                var attachmentIdList = await _pmsdbContext.Attachment.Where(x => x.IsActive).ToListAsync();



                var attachList = _pmsdbContext.Attachment
                                .AsNoTracking()
                                .Where(a => a.IsActive && a.ProjectCreationId == model.ProjectCreationId)
                                .AsEnumerable()
                                .Join(
                                    users,
                                    a => a.CreatedBy,
                                    u => u.UserId,
                                    (a, u) => new AttachmentViewModel
                                    {
                                        AttachmentId = a.AttachmentId,
                                        ProjectCreationId = a.ProjectCreationId,
                                        MileStoneId = a.MileStoneId,
                                        AttachmentType = a.AttachmentType,
                                        Filename = a.Filename,
                                        FileLocation = a.FileLocation,
                                        CreatedBy = a.CreatedBy,
                                        CreatedName = u.Fullname,
                                        DateCreated = a.DateCreated,
                                        IsActive = a.IsActive
                                    }
                                )
                                .OrderByDescending(x => x.AttachmentId)
                                .ToList();


                var result = (from m in milestone
                              join u in users on m.CreatedBy equals u.UserId
                              join s in _dbContext.Status.AsNoTracking() on m.Status equals s.StatusId
                              join paystat in _dbContext.Status.AsNoTracking() on m.PaymentStatus equals paystat.StatusId
                              where m.ProjectCreationId == model.ProjectCreationId
                              select new MilestoneViewModel
                              {
                                  MileStoneId = m.MileStoneId,
                                  ProjectCreationId = m.ProjectCreationId,
                                  MilestoneName = m.MilestoneName,
                                  Percentage = m.Percentage,
                                  TargetCompletedDate = m.TargetCompletedDate,
                                  ActualCompletionDate = m.ActualCompletionDate,
                                  PaymentStatus = m.PaymentStatus,
                                  TargetCompletedDateString = m.TargetCompletedDate.Value.ToString("MM/dd/yyyy"),
                                  ActualCompletionDateString = m.ActualCompletionDate.Value.ToString("MM/dd/yyyy"),
                                  Status = m.Status,
                                  StatusName = s.StatusName,
                                  PaymentStatusName = paystat.StatusName,
                                  DateCreated = m.DateCreated,
                                  CreatedBy = m.CreatedBy,
                                  DateModified = m.DateModified,
                                  ModifiedBy = m.ModifiedBy,
                                  IsActive = m.IsActive,
                                  Remarks = m.Remarks,
                                  AttachmentListId = attachmentIdList.Where(x => x.MileStoneId == m.MileStoneId).Select(x => x.AttachmentId).ToList(),
                                  AttachmentList = attachList.Where(x => x.MileStoneId == m.MileStoneId).ToList()
                              }).ToList();

                model.MilestoneList = result;
                model.StatusList = statusList;

                return model;

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

        public async Task SaveMilestone(MilestoneViewModel model)
        {
            try
            {
                if (model.MileStoneId == 0)
                {
                    Milestone entity = new Milestone();
                    entity.ProjectCreationId = model.ProjectCreationId;
                    entity.MilestoneName = model.MilestoneName;
                    entity.Percentage = model.Percentage;
                    entity.TargetCompletedDate = model.TargetCompletedDate;
                    entity.ActualCompletionDate = model.ActualCompletionDate;
                    entity.PaymentStatus = model.PaymentStatus;
                    entity.Status = model.Status;
                    entity.CreatedBy = model.CreatedBy;
                    entity.DateCreated = DateTime.Now;
                    entity.ModifiedBy = null;
                    entity.DateModified = null;
                    entity.IsActive = true;
                    entity.Remarks = model.Remarks;
                    await _pmsdbContext.Milestone.AddAsync(entity);
                    await _pmsdbContext.SaveChangesAsync();
                    model.MileStoneId = entity.MileStoneId;

                    await SaveFileMilestone(model);

                    ProjectViewModel projModel = new ProjectViewModel();
                    projModel.ProjectCreationId = model.ProjectCreationId;
                    await GetMilestoneByProjectId(projModel);
                    // return (StatusCodes.Status200OK, "Successfully saved");
                }
                else
                {
                    var entity = await _pmsdbContext.Milestone.FirstOrDefaultAsync(x => x.MileStoneId == model.MileStoneId);
                    entity.MilestoneName = model.MilestoneName;
                    entity.Percentage = model.Percentage;
                    entity.TargetCompletedDate = model.TargetCompletedDate;
                    entity.ActualCompletionDate = model.ActualCompletionDate;
                    entity.PaymentStatus = model.PaymentStatus;
                    entity.Status = model.Status;
                    entity.DateModified = DateTime.Now;
                    entity.ModifiedBy = model.ModifiedBy;
                    entity.IsActive = true;
                    entity.Remarks = model.Remarks;
                    _pmsdbContext.Milestone.Entry(entity).State = EntityState.Modified;
                    await _pmsdbContext.SaveChangesAsync();

                    await SaveFileMilestone(model);

                    ProjectViewModel projModel = new ProjectViewModel();
                    projModel.ProjectCreationId = model.ProjectCreationId;
                    await GetMilestoneByProjectId(projModel);
                    // return (StatusCodes.Status200OK, "Successfully updated");
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                //  return (StatusCodes.Status406NotAcceptable, ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        private async Task SaveFileMilestone(MilestoneViewModel model)
        {
            try
            {
                string fileloc = @"C:\\DCI App\\PMS\\" + model.ProjectCreationId.ToString() + @"\";

                if (!Directory.Exists(fileloc))
                    Directory.CreateDirectory(fileloc);



                if (model.OtherAttachmentMilestone != null && model.OtherAttachmentMilestone.Any())
                {
                    foreach (var file in model.OtherAttachmentMilestone)
                    {
                        string _fileloc = fileloc + file.FileName;

                        await SaveAttachment(new AttachmentViewModel
                        {
                            ProjectCreationId = model.ProjectCreationId,
                            MileStoneId = model.MileStoneId,
                            DeliverableId = 0,
                            AttachmentType = (int)EnumAttachmentType.MILESTONE,
                            Filename = file.FileName,
                            FileLocation = _fileloc,
                            CreatedBy = model.CreatedBy
                        });
                    }
                }


            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public async Task<MilestoneViewModel> GetDeliverablesByMilestoneId(MilestoneViewModel model)
        {

            try
            {
                var users = await _dbContext.User
                                        .AsNoTracking()
                                        .Where(p => p.IsActive)
                                        .Select(u => new
                                        {
                                            u.UserId,
                                            u.Fullname
                                        })
                                        .ToListAsync();

                var deliverable = _pmsdbContext.Deliverable
                                        .AsNoTracking()
                                        .Where(p => p.IsActive).ToList();

                var attachmentIdList = await _pmsdbContext.Attachment.Where(x => x.IsActive).ToListAsync();

                var statusList = await _dbContext.Status
                  .AsNoTracking()
                  .Where(p => p.IsActive)
                  .Select(u => new StatusViewModel
                  {
                      StatusId = u.StatusId,
                      StatusName = u.StatusName
                  })
                  .ToListAsync();


                var attachList = _pmsdbContext.Attachment
                         .AsNoTracking()
                         .Where(a => a.IsActive && a.MileStoneId == model.MileStoneId)
                         .AsEnumerable()
                         .Join(
                             users,
                             a => a.CreatedBy,
                             u => u.UserId,
                             (a, u) => new AttachmentViewModel
                             {
                                 AttachmentId = a.AttachmentId,
                                 ProjectCreationId = a.ProjectCreationId,
                                 MileStoneId = a.MileStoneId,
                                 DeliverableId = a.DeliverableId,
                                 AttachmentType = a.AttachmentType,
                                 Filename = a.Filename,
                                 FileLocation = a.FileLocation,
                                 CreatedBy = a.CreatedBy,
                                 CreatedName = u.Fullname,
                                 DateCreated = a.DateCreated,
                                 IsActive = a.IsActive
                             }
                         )
                         .OrderByDescending(x => x.AttachmentId)
                         .ToList();

                var result = (from m in deliverable
                              join u in users on m.CreatedBy equals u.UserId
                              join s in _dbContext.Status.AsNoTracking() on m.Status equals s.StatusId
                              where m.MileStoneId == model.MileStoneId
                              select new DeliverableViewModel
                              {
                                  DeliverableId = m.DeliverableId,
                                  MileStoneId = m.MileStoneId,
                                  DeliverableName = m.DeliverableName,
                                  Status = m.Status,
                                  StatusName = s.StatusName,
                                  DateCreated = m.DateCreated,
                                  CreatedBy = m.CreatedBy,
                                  DateModified = m.DateModified,
                                  ModifiedBy = m.ModifiedBy,
                                  IsActive = m.IsActive,
                                  AttachmentListId = attachmentIdList.Where(x => x.DeliverableId == m.DeliverableId).Select(x => x.AttachmentId).ToList(),
                                  AttachmentList = attachList.Where(x => x.DeliverableId == m.DeliverableId).ToList()
                              }).ToList();


                model.DeliverableList = result;
                model.StatusList = statusList;

                return model;

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

        public async Task SaveDeliverable(DeliverableViewModel model)
        {
            try
            {
                if (model.DeliverableId == 0)
                {
                    Deliverable entity = new Deliverable();
                    entity.DeliverableId = model.DeliverableId;
                    entity.MileStoneId = model.MileStoneId;
                    entity.DeliverableName = model.DeliverableName;
                    entity.Status = model.Status;
                    entity.CreatedBy = model.CreatedBy;
                    entity.DateCreated = DateTime.Now;
                    entity.ModifiedBy = null;
                    entity.DateModified = null;
                    entity.IsActive = true;
                    await _pmsdbContext.Deliverable.AddAsync(entity);
                    await _pmsdbContext.SaveChangesAsync();

                    await SaveFileDeliverable(model);

                    MilestoneViewModel miles = new MilestoneViewModel();
                    miles.MileStoneId = model.MileStoneId;
                    await GetDeliverablesByMilestoneId(miles);
                }
                else
                {
                    var entity = await _pmsdbContext.Deliverable.FirstOrDefaultAsync(x => x.MileStoneId == model.MileStoneId);
                    entity.DeliverableName = model.DeliverableName;
                    entity.Status = model.Status;
                    entity.DateModified = DateTime.Now;
                    entity.ModifiedBy = model.ModifiedBy;
                    entity.IsActive = true;

                    _pmsdbContext.Deliverable.Entry(entity).State = EntityState.Modified;
                    await _pmsdbContext.SaveChangesAsync();

                    await SaveFileDeliverable(model);

                    MilestoneViewModel miles = new MilestoneViewModel();
                    miles.MileStoneId = model.MileStoneId;
                    await GetDeliverablesByMilestoneId(miles);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                // return (StatusCodes.Status406NotAcceptable, ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private async Task SaveFileDeliverable(DeliverableViewModel model)
        {
            try
            {
                string fileloc = @"C:\\DCI App\\PMS\\" + model.MileStoneId.ToString() + @"\";

                if (!Directory.Exists(fileloc))
                    Directory.CreateDirectory(fileloc);



                if (model.OtherAttachmentDeliverable != null && model.OtherAttachmentDeliverable.Any())
                {
                    foreach (var file in model.OtherAttachmentDeliverable)
                    {
                        string _fileloc = fileloc + file.FileName;

                        await SaveAttachment(new AttachmentViewModel
                        {
                            ProjectCreationId = model.ProjectCreationId,
                            MileStoneId = model.MileStoneId,
                            DeliverableId = model.DeliverableId,
                            AttachmentType = (int)EnumAttachmentType.DELIVERABLES,
                            Filename = file.FileName,
                            FileLocation = _fileloc,
                            CreatedBy = model.CreatedBy
                        });
                    }
                }


            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public async Task<(int statuscode, string message)> DeleteProject(ProjectViewModel model)
        {
            try
            {
                var entity = await _pmsdbContext.Project.FirstOrDefaultAsync(x => x.ProjectCreationId == model.ProjectCreationId && x.IsActive == true);
                if (entity == null)
                {
                    return (StatusCodes.Status406NotAcceptable, "Invalid Project Id");
                }

                entity.IsActive = false;
                entity.ModifiedBy = model.ModifiedBy;
                entity.DateModified = DateTime.Now;
                _pmsdbContext.Project.Entry(entity).State = EntityState.Modified;
                await _pmsdbContext.SaveChangesAsync();
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

        public async Task<(int statuscode, string message)> DeleteMilestone(MilestoneViewModel model)
        {
            try
            {
                var entity = await _pmsdbContext.Milestone.FirstOrDefaultAsync(x => x.MileStoneId == model.MileStoneId && x.IsActive == true);
                if (entity == null)
                {
                    return (StatusCodes.Status406NotAcceptable, "Invalid Milestone Id");
                }

                entity.IsActive = false;
                entity.ModifiedBy = model.ModifiedBy;
                entity.DateModified = DateTime.Now;
                _pmsdbContext.Milestone.Entry(entity).State = EntityState.Modified;
                await _pmsdbContext.SaveChangesAsync();
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
        public async Task<(int statuscode, string message)> DeleteDeliverable(DeliverableViewModel model)
        {
            try
            {
                var entity = await _pmsdbContext.Deliverable.FirstOrDefaultAsync(x => x.DeliverableId == model.DeliverableId && x.IsActive == true);
                if (entity == null)
                {
                    return (StatusCodes.Status406NotAcceptable, "Invalid Deliverable Id");
                }

                entity.IsActive = false;
                entity.ModifiedBy = model.ModifiedBy;
                entity.DateModified = DateTime.Now;
                _pmsdbContext.Deliverable.Entry(entity).State = EntityState.Modified;
                await _pmsdbContext.SaveChangesAsync();
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


        private async Task<string> GenereteRequestNo()
        {

            try
            {
                int _currentYear = DateTime.Now.Year;
                int _currentMonth = DateTime.Now.Month;
                var _leaveContext = await _pmsdbContext.Project
                                                .Where(x => x.IsActive == true && x.DateCreated.Date.Year == _currentYear && x.DateCreated.Date.Month == _currentMonth)
                                                .AsNoTracking()
                                                .ToListAsync();


                int totalrecords = _leaveContext.Count() + 1;
                string finalSetRecords = FormatHelper.GetFormattedRequestNo(totalrecords);
                string yearMonth = DateTime.Now.ToString("yyyyMM");
                string req = Constants.ModuleCode_PMS;

                return $"{req}-{yearMonth}-{finalSetRecords}";
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
            return string.Empty;
        }

        public async Task<(int statuscode, string message)> DeleteAttachment(AttachmentViewModel model)
        {
            try
            {
                var entity = await _pmsdbContext.Attachment.FirstOrDefaultAsync(x => x.AttachmentId == model.AttachmentId);
                if (entity == null)
                {
                    return (StatusCodes.Status406NotAcceptable, "Invalid Attachment Id");
                }

                entity.IsActive = false;
                _pmsdbContext.Attachment.Entry(entity).State = EntityState.Modified;
                await _pmsdbContext.SaveChangesAsync();
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


        private async Task SaveAttachment(AttachmentViewModel model)
        {
            var sql = @"
                    INSERT INTO dbo.Attachment
                    (
                        ProjectCreationId,
                        MileStoneId,
                        DeliverableId,
                        AttachmentType,
                        Filename,
                        FileLocation,
                        DateCreated,
                        CreatedBy,
                        IsActive
                    )
                    VALUES
                    (
                        @ProjectCreationId,
                        @MileStoneId,
                        @DeliverableId,
                        @AttachmentType,
                        @Filename,
                        @FileLocation,
                        @DateCreated,
                        @CreatedBy,
                        1
                    )";

            await _connection.ExecuteAsync(sql, new
            {
                model.ProjectCreationId,
                model.MileStoneId,
                model.DeliverableId,
                model.AttachmentType,
                model.Filename,
                model.FileLocation,
                DateCreated = DateTime.Now,
                model.CreatedBy
            });
        }

        public async Task<AttachmentViewModel> ViewFile(AttachmentViewModel model)
        {
            try
            {
                var entity = await _pmsdbContext.Attachment.FirstOrDefaultAsync(x => x.AttachmentId == model.AttachmentId && x.IsActive == true);


                 var result = new AttachmentViewModel
                 {
                     AttachmentId = entity.AttachmentId,
                     ProjectCreationId = entity.ProjectCreationId,
                     MileStoneId = entity.MileStoneId,
                     DeliverableId = entity.DeliverableId,
                     AttachmentType = entity.AttachmentType,
                     Filename = entity.Filename,
                     FileLocation = entity.FileLocation,
                     DateCreated = entity.DateCreated,
                     CreatedBy = entity.CreatedBy,
                     IsActive = entity.IsActive
                 };

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());               
            }
            finally
            {
               
                Log.CloseAndFlush();
            }
            return new AttachmentViewModel();
           
        }

    }
}
