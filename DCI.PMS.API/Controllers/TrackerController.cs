using DCI.PMS.Models.ViewModel;
using DCI.PMS.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DCI.PMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrackerController : Controller
    {
        ITrackerRepository _trackerRepository;
        public TrackerController(ITrackerRepository trackerRepository)
        {
            _trackerRepository = trackerRepository;
        }

        [HttpPost]
        [Route("GetAllTracker")]
        public async Task<IActionResult> GetAllTracker([FromForm] TrackerViewModel model)
        {
            return Ok(await _trackerRepository.GetAllTracker());
        }
    }
}
