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
        public async Task<IActionResult> GetAllProject([FromBody] ProjectViewModel model)
        {
            return Ok(await _projectRepository.GetAllProject());
        }
    }
}
