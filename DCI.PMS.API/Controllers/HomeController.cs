using DCI.Models.ViewModel;
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
        public async Task<IActionResult> GetDashboard([FromForm] PMSDashboardViewModel model)
        {
            return Ok(await _homeRepository.GetDashboard());
        }

        [HttpPost]
        [Route("GetAllNotification")]
        public async Task<IActionResult> GetAllNotification(NotificationViewModel model)
        {
            return Ok(await _homeRepository.GetAllNotification(model));
        }

        [HttpPost]
        [Route("MarkAsRead")]
        public async Task<IActionResult> MarkAsRead([FromBody] NotificationViewModel model)
        {
            var markAsRead = _homeRepository.MarkAsRead(model);
            return Ok();
        }

    }
}
