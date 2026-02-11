using DCI.PMS.Models.ViewModel;
using DCI.PMS.Repository;
using DCI.PMS.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DCI.PMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        IHomeRepository _homeRepository;

        public HomeController(IHomeRepository homeRepository)
        {
            _homeRepository = homeRepository;
        }
        [HttpPost]
        [Route("GetDashboard")]
        public async Task<IActionResult> GetDashboard([FromForm] DashboardViewModel model)
        {
            return Ok(await _homeRepository.GetDashboard());
        }
    }
}
