using DCI.Core.Common;
using DCI.Core.Helpers;
using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Serilog;
using System;

namespace DCI.Repositories
{
	public class JobApplicationRepository : IJobApplicationRepository, IDisposable
	{

		private DCIdbContext _dbContext;
		public JobApplicationRepository(DCIdbContext context)
		{
			this._dbContext = context;
		}
		public void Dispose()
		{
			_dbContext.Dispose();
		}
		public async Task<JobApplicant> GetJobApplicantById(int jobApplicantId)
		{
			return await _dbContext.JobApplicant.FindAsync(jobApplicantId);
		}
		public async Task<IList<JobApplicationViewModel>> GetAllJobApplicant()
		{
		
			//Type enumType = typeof(EnumApplicantStatus);
			var result = await _dbContext.JobApplicant.AsQueryable().Where(x => x.IsActive == true).ToListAsync();
			var query = from job in result
						select new JobApplicationViewModel
						{
							JobApplicantId = job.JobApplicantId,
							ApplicantName = job.ApplicantName,
							ContactNumber = job.ContactNumber,
							Email = job.Email,
							Address = job.Address,
							DateofBirth = job.DateofBirth,
							JobSite = job.JobSite,
							PositionOffer = job.PositionOffer,
							StatusName = EnumHelper.GetEnumDescriptionByTypeValue(typeof(EnumApplicantStatus), job.Status),
							Status = job.Status,
						};
			return query.ToList();		
		}

		public async Task<(int statuscode, string message)> Save(JobApplicationViewModel model)
		{

			try
			{
				if (model.JobApplicantId == 0)
				{
					JobApplicant entity = new JobApplicant();
					entity.JobApplicantId = model.JobApplicantId;
					entity.ApplicantName = model.ApplicantName;
					entity.ContactNumber = model.ContactNumber;
					entity.Email = model.Email;
					entity.Address = model.Address;
					entity.DateofBirth = model.DateofBirth.HasValue ? model.DateofBirth.Value : null;
					entity.PositionOffer = model.PositionOffer;
					entity.JobSite = model.JobSite;
					entity.Status = model.ActionType == 1 ? (int)EnumApplicantStatus.Save : (int)EnumApplicantStatus.ForInitialInterview;
					entity.CreatedBy = model.CreatedBy;
					entity.DateCreated = DateTime.Now;
					entity.ModifiedBy = 0;
					entity.DateModified = DateTime.Now;
					entity.IsActive = true;
					await _dbContext.JobApplicant.AddAsync(entity);
					await _dbContext.SaveChangesAsync();
					return (StatusCodes.Status200OK, "Successfully saved");
				}
				else
				{
					var entity = await _dbContext.JobApplicant.FirstOrDefaultAsync(x => x.JobApplicantId == model.JobApplicantId);
					entity.JobApplicantId = model.JobApplicantId;
					entity.ApplicantName = model.ApplicantName;
					entity.ContactNumber = model.ContactNumber;
					entity.Email = model.Email;
					entity.Address = model.Address;
					entity.DateofBirth = model.DateofBirth.HasValue ? model.DateofBirth.Value : null;
					entity.PositionOffer = model.PositionOffer;
					entity.JobSite = model.JobSite;
					entity.Status = model.ActionType == 1 ? (int)EnumApplicantStatus.Save : (int)EnumApplicantStatus.ForInitialInterview;
					entity.DateCreated = entity.DateCreated;
					entity.CreatedBy = entity.CreatedBy;
					entity.DateModified = DateTime.Now;
					entity.ModifiedBy = model.ModifiedBy;
					entity.IsActive = true;

					_dbContext.JobApplicant.Entry(entity).State = EntityState.Modified;
					await _dbContext.SaveChangesAsync();
					return (StatusCodes.Status200OK, "Successfully updated");
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
	}
}
