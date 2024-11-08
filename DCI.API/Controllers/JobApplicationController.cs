using DCI.Data;
using DCI.Models.ViewModel;
using DCI.Repositories;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DCI.API.Controllers
{
	//[ApiController]
	//[Route("api/[controller]")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class JobApplicationController : Controller
	{
		private readonly DCIdbContext _dcIdbContext;
		IJobApplicationRepository _jobApplicationRepository;
		public JobApplicationController(IJobApplicationRepository jobApplicationRepository)
		{
			this._jobApplicationRepository = jobApplicationRepository;
		}

		#region Job Applicant

		[HttpGet]
		[Route("JobApplicant")]
		public async Task<IActionResult> JobApplicant()
		{
			return Ok(await _jobApplicationRepository.GetAllJobApplicant());
		}

		[HttpPost]
		[Route("GetJobApplicantById")]
		public async Task<IActionResult> GetJobApplicantById([FromBody] JobApplicationViewModel model)
		{
			return Ok(await _jobApplicationRepository.GetJobApplicantById(model.JobApplicantId));		   
		}

		[HttpPost]
		[Route("SaveJobApplicant")]
		public async Task<IActionResult> SaveJobApplicant([FromBody] JobApplicationViewModel model)
		{
			var result = await _jobApplicationRepository.Save(model);
			return StatusCode(result.statuscode, result.message);
		}
		#endregion
	}
}
