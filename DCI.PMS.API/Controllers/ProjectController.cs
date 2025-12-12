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

        [HttpGet]
        [Route("GetAllProject")]
        public async Task<IActionResult> GetAllProject()
        {
            var result = await _projectRepository.GetAllProject();
            return Ok(result);
        }
    }
}
