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
        //public async Task<IActionResult> SaveProject([FromBody] ProjectViewModel model)
        public async Task<IActionResult> SaveProject(ProjectViewModel model)
        {
            //var result = await _projectRepository.SaveProject(model);
            //return StatusCode(result.statuscode, result.message);
          
            return Ok( _projectRepository.SaveProject(model));
        }

                        
        [HttpPost]
        [Route("GetMilestoneByProjectId")]
        public async Task<IActionResult> GetMilestoneByProjectId([FromBody] ProjectViewModel model)
        {
            return Ok(await _projectRepository.GetMilestoneByProjectId(model));
        }

        [HttpPost]
        [Route("GetDeliverablesByMilestoneId")]
        public async Task<IActionResult> GetDeliverablesByMilestoneId([FromBody] MilestoneViewModel model)
        {
            return Ok(await _projectRepository.GetDeliverablesByMilestoneId(model));
        }

        
        [HttpPost]
        [Route("SaveMilestone")]
        public async Task<IActionResult> SaveMilestone([FromForm] MilestoneViewModel model)
        {
            await _projectRepository.SaveMilestone(model);
            return Ok();
           // return StatusCode(result.statuscode, result.message);
        }

        [HttpPost]
        [Route("SaveDeliverable")]
        public async Task<IActionResult> SaveDeliverable([FromBody] DeliverableViewModel model)
        {
            await _projectRepository.SaveDeliverable(model);
            return Ok();
            // return StatusCode(result.statuscode, result.message);
        }

        [HttpPost]
        [Route("DeleteProject")]
        public async Task<IActionResult> DeleteProject([FromBody] ProjectViewModel model)
        {
            await _projectRepository.DeleteProject(model);
            return Ok(); 
        }

        [HttpPost]
        [Route("DeleteMilestone")]
        public async Task<IActionResult> DeleteMilestone([FromBody] MilestoneViewModel model)
        {
            //await _projectRepository.DeleteMilestone(model);
            //return Ok();
            var result = await _projectRepository.DeleteMilestone(model);
            return StatusCode(result.statuscode, result.message);
        }

        [HttpPost]
        [Route("DeleteDeliverable")]
        public async Task<IActionResult> DeleteDeliverable([FromBody] DeliverableViewModel model)
        {
            await _projectRepository.DeleteDeliverable(model);
            return Ok();
        }

        [HttpPost]
        [Route("DeleteAttachment")]
        public async Task<IActionResult> DeleteAttachment([FromBody] AttachmentViewModel model)
        {            
            var result = await _projectRepository.DeleteAttachment(model);
            return StatusCode(result.statuscode, result.message);
        }
    }
}
