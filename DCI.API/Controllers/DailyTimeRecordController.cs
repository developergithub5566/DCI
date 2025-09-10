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
        IOvertimeRepository _overtimeRepository;
        IWfhRepository _wfhRepository;

        public DailyTimeRecordController(IDailyTimeRecordRepository dtrRepository, ILeaveRepository leaveRepository,
            IOvertimeRepository overtimeRepository, IWfhRepository wfhRepository)
        {
            _dtrRepository = dtrRepository;
            _leaveRepository = leaveRepository;
            _overtimeRepository = overtimeRepository;
            _wfhRepository = wfhRepository;
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
        [Route("GetUndertimeById")]
        public async Task<IActionResult> GetUndertimeById([FromBody] DailyTimeRecordViewModel model)
        {
            return Ok(await _dtrRepository.GetUndertimeById(model));
        }

        [HttpPost]
        [Route("SaveUndertime")]
        public async Task<IActionResult> SaveUndertime([FromBody] List<UndertimeDeductionViewModel> model)
        {
            return Ok(await _dtrRepository.SaveUndertime(model));
        }

        [HttpPost]
        [Route("GetAllWFH")]
        public async Task<IActionResult> GetAllWFH([FromBody] DailyTimeRecordViewModel model)
        {
            return Ok(await _wfhRepository.GetAllWFH(model));
        }

        [HttpPost]
        [Route("SaveWFHTimeIn")]
        public async Task<IActionResult> SaveWFHTimeIn([FromBody] WFHViewModel model)
        {
            return Ok(await _wfhRepository.SaveWFHTimeIn(model));
        }

        [HttpPost]
        [Route("GetAllWFHApplication")]
        public async Task<IActionResult> GetAllWFHApplication([FromBody] WFHHeaderViewModel model)
        {
            return Ok(await _wfhRepository.GetAllWFHApplication(model));
        }


        [HttpPost]
        [Route("GetWFHApplicationDetailByWfhHeaderId")]
        public async Task<IActionResult> GetWFHApplicationDetailByWfhHeaderId([FromBody] WFHHeaderViewModel model)
        {
            return Ok(await _wfhRepository.GetWFHApplicationDetailByWfhHeaderId(model));
        }
     
        [HttpPost]
        [Route("GetWFHLogsByEmployeeId")]
        public async Task<IActionResult> GetWFHLogsByEmployeeId([FromBody] WFHViewModel model)
        {
            return Ok(await _wfhRepository.GetWFHLogsByEmployeeId(model));
        }

        [HttpPost]
        [Route("SaveWFHApplication")]
        public async Task<IActionResult> SaveWFHApplication([FromBody] WfhApplicationViewModel param)
        {
            return Ok(await _wfhRepository.SaveWFHApplication(param));
        }

        [HttpPost]
        [Route("Overtime")]
        public async Task<IActionResult> Overtime([FromBody] OvertimeViewModel model)
        {
            return Ok(await _overtimeRepository.Overtime(model));
        }

        [HttpPost]
        [Route("AddOvertime")]
        public async Task<IActionResult> AddOvertime([FromBody] OvertimeViewModel model)
        {
            return Ok(await _overtimeRepository.AddOvertime(model));
        }


        [HttpPost]
        [Route("GetAllAttendanceByDate")]
        public async Task<IActionResult> GetAllAttendanceByDate([FromBody] OvertimeViewModel model)
        {
            return Ok(await _overtimeRepository.GetAllAttendanceByDate(model));
        }

        [HttpPost]
        [Route("SaveOvertime")]
        public async Task<IActionResult> SaveOvertime([FromBody] OvertimeViewModel param)
        {
            return Ok(await _overtimeRepository.SaveOvertime(param));
        }

      


    }
}
