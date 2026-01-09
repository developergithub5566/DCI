using DCI.Models.ViewModel;
using DCI.PMS.Models.ViewModel;
using DCI.PMS.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DCI.PMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : Controller
    {

        IProjectRepository _projectRepository;
        public ProjectController(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        [HttpPost]
        [Route("GetAllProject")]
        public async Task<IActionResult> GetAllProject([FromForm] ProjectViewModel model)
        {
            return Ok(await _projectRepository.GetAllProject());
        }

        [HttpPost]
        [Route("GetProjectById")]
        public async Task<IActionResult> GetProjectById([FromBody] ProjectViewModel model)
        {
            return Ok(await _projectRepository.GetProjectById(model));
        }



        [HttpPost]
        [Route("SaveProject")]
        public async Task<IActionResult> SaveProject([FromBody] ProjectViewModel model)
        {
            var result = await _projectRepository.SaveProject(model);
            return StatusCode(result.statuscode, result.message);
        }


        [HttpPost]
        [Route("GetMilestoneByProjectId")]
        public async Task<IActionResult> GetMilestoneByProjectId([FromBody] ProjectViewModel model)
        {
            return Ok(await _projectRepository.GetMilestoneByProjectId(model));
        }

        [HttpPost]
        [Route("SaveMilestone")]
        public async Task<IActionResult> SaveMilestone([FromBody] MilestoneViewModel model)
        {
            var result = await _projectRepository.SaveMilestone(model);
            return StatusCode(result.statuscode, result.message);
        }
    }
}
