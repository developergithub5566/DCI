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
        [Route("SaveProject")]
        public async Task<IActionResult> SaveProject([FromBody] ProjectViewModel model)
        {
            var result = await _projectRepository.Save(model);
            return StatusCode(result.statuscode, result.message);
        }
    }
}
