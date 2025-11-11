using DCI.Data;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DCI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        private readonly DCIdbContext _dcIdbContext;
        IHomeRepository _homeRepository;

        public HomeController(IHomeRepository homeRepository)
        {
            _homeRepository = homeRepository;
        }

        [HttpPost]
        [Route("GetAllAnnouncement")]
        public async Task<IActionResult> GetAllAnnouncement(DashboardViewModel model)
        {
            var result = await _homeRepository.GetAllAnnouncement(model);
            return Ok(result);
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

        [HttpPost]
        [Route("SaveEmailNotificationForBiometrics")]
        public async Task<IActionResult> SaveEmailNotificationForBiometrics([FromBody] UserViewModel model)
        {
            await _homeRepository.SaveEmailNotificationForBiometrics(model);

            return Ok();
           
        }

        [HttpPost]
        [Route("SaveEmailNotificationForAttendanceConfirmation")]
        public async Task<IActionResult> SaveEmailNotificationForAttendanceConfirmation([FromBody] UserViewModel model)
        {
            await _homeRepository.SaveEmailNotificationForAttendanceConfirmation(model);

            return Ok();

        }

    }
}

