using DCI.Models.Entities;
using DCI.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Repositories.Interface
{
	public interface IJobApplicationRepository : IDisposable
	{
		Task<JobApplicant> GetJobApplicantById(int jobApplicantId);
		Task<IList<JobApplicationViewModel>> GetAllJobApplicant();
		Task<(int statuscode, string message)> Save(JobApplicationViewModel model);
	}
}
