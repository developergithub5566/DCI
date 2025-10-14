using DCI.Models.ViewModel;
using DCI.Trigger.API.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Serilog;
using System.Text;

namespace DCI.Trigger.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : Controller
    {
        ITriggerRepository _triggerRepository;

        public AttendanceController(ITriggerRepository triggerRepository)
        {
            _triggerRepository = triggerRepository;
        }

        [HttpGet]
        [Route("GetAllAttendanceLogs")]
        public async Task<IActionResult> GetAllAttendanceLogs()
        {
            try
            {
                var result = await _triggerRepository.GetAllAttendanceLog();
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return BadRequest(ex.Message);
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        [HttpPost]
        [Route("UpdateOutBoxQueueStatus")]
        public async Task<IActionResult> UpdateOutBoxQueueStatus(OutboxMessageViewModel model)
        {
            try
            {
                await _triggerRepository.UpdateOutBoxQueueStatus(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return Json(new { success = false, message = ex.Message });
            }
            finally
            {
                Log.CloseAndFlush();
            }
            return Json(new { success = false, message = "An error occurred. Please try again." });
        }
    }
}
