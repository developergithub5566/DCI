using DCI.Data;
using DCI.Models.ViewModel;
using DCI.Repositories;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DCI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class DailyTimeRecordController : Controller
    {
        private readonly DCIdbContext _dcIdbContext;
        IDailyTimeRecordRepository _dtrRepository;
        ILeaveRepository _leaveRepository;

        public DailyTimeRecordController(IDailyTimeRecordRepository dtrRepository, ILeaveRepository leaveRepository)
        {
            _dtrRepository = dtrRepository;
            _leaveRepository = leaveRepository;
        }

        [HttpPost]
        [Route("GetAllDTR")]
        public async Task<IActionResult> GetAllDTR(DailyTimeRecordViewModel model)
        {
            var result = await _dtrRepository.GetAllDTR(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetAllDTRByEmpNo")]
        public async Task<IActionResult> GetAllDTRByEmpNo([FromBody] DailyTimeRecordViewModel model)
        {
            return Ok(await _dtrRepository.GetAllDTRByEmpNo(model.EMPLOYEE_NO));
        }

        [HttpPost]
        [Route("GetAllLeave")]
        public async Task<IActionResult> GetAllLeave([FromBody] LeaveViewModel model)
        {
            return Ok(await _leaveRepository.GetAllLeave(model));
        }

        [HttpPost]
        [Route("RequestLeave")]
        public async Task<IActionResult> RequestLeave([FromBody] LeaveViewModel model)
        {
            //LeaveViewModel model = new LeaveViewModel();
            //var result = await _leaveRepository.RequestLeave(model);
            // return Ok(result);
            return Ok(await _leaveRepository.RequestLeave(model));
        }

        [HttpPost]
        [Route("SaveLeave")]
        public async Task<IActionResult> SaveLeave([FromBody] LeaveFormViewModel model)
        {
            return Ok(await _leaveRepository.SaveLeave(model));
        }

        [HttpPost]
        [Route("GetAllDTRCorrection")]
        public async Task<IActionResult> GetAllDTRCorrection([FromBody] DTRCorrectionViewModel model)
        {
            return Ok(await _dtrRepository.GetAllDTRCorrection(model));
        }

        [HttpPost]
        [Route("DTRCorrectionById")]
        public async Task<IActionResult> DTRCorrectionById([FromBody] DTRCorrectionViewModel model)
        {
            return Ok(await _dtrRepository.DTRCorrectionByDtrId(model.DtrId));
        }

        [HttpPost]
        [Route("SaveDTRCorrection")]
        public async Task<IActionResult> SaveDTRCorrection([FromBody] DTRCorrectionViewModel model)
        {
            return Ok(await _dtrRepository.SaveDTRCorrection(model));
        }

        [HttpPost]
        [Route("GetAllUndertime")]
        public async Task<IActionResult> GetAllUndertime(DailyTimeRecordViewModel model)
        {
            var result = await _dtrRepository.GetAllUndertime(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("UndertimeById")]
        public async Task<IActionResult> UndertimeById([FromBody] DailyTimeRecordViewModel model)
        {
            return Ok(await _dtrRepository.UndertimeById(model));
        }
    }
}
