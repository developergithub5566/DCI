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
        IUndertimeRepository _undertimeRepository;
        ILateRepository _lateRepository;

        public DailyTimeRecordController(IDailyTimeRecordRepository dtrRepository, ILeaveRepository leaveRepository,
            IOvertimeRepository overtimeRepository, IWfhRepository wfhRepository, IUndertimeRepository undertimeRepository, ILateRepository lateRepository)
        {
            _dtrRepository = dtrRepository;
            _leaveRepository = leaveRepository;
            _overtimeRepository = overtimeRepository;
            _wfhRepository = wfhRepository;
            _undertimeRepository = undertimeRepository;
            _lateRepository = lateRepository;
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
        [Route("GetBiometricLogsByEmployeeId")]
        public async Task<IActionResult> GetBiometricLogsByEmployeeId([FromBody] DailyTimeRecordViewModel model)
        {
            return Ok(await _dtrRepository.GetBiometricLogsByEmployeeId(model));
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
            // return Ok(await _leaveRepository.SaveLeave(model));
            var result = await _leaveRepository.SaveLeave(model);
            return StatusCode(result.statuscode, result.message);
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
        [Route("CancelDTRCorrection")]
        public async Task<IActionResult> CancelDTRCorrection([FromBody] DTRCorrectionViewModel model)
        {
            var result = await _dtrRepository.CancelDTRCorrection(model);
            return StatusCode(result.statuscode, result.message);
        }

        [HttpPost]
        [Route("SaveDTRCorrection")]
        public async Task<IActionResult> SaveDTRCorrection([FromBody] DTRCorrectionViewModel model)
        {
            // return Ok(await _dtrRepository.SaveDTRCorrection(model));
            var result = await _dtrRepository.SaveDTRCorrection(model);
            return StatusCode(result.statuscode, result.message);
        }

        [HttpPost]
        [Route("GetAllUndertime")]
        public async Task<IActionResult> GetAllUndertime(DailyTimeRecordViewModel model)
        {
            var result = await _undertimeRepository.GetAllUndertime(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetUndertimeById")]
        public async Task<IActionResult> GetUndertimeById([FromBody] DailyTimeRecordViewModel model)
        {
            return Ok(await _undertimeRepository.GetUndertimeById(model));
        }

        [HttpPost]
        [Route("SaveUndertime")]
        public async Task<IActionResult> SaveUndertime([FromBody] UndertimeHeaderDeductionViewModel model)
        {
            // return Ok(await _dtrRepository.SaveUndertime(model));
            var result = await _undertimeRepository.SaveUndertime(model);
            return StatusCode(result.statuscode, result.message);
        }

        [HttpPost]
        [Route("GetUndertimeDeduction")]
        public async Task<IActionResult> GetUndertimeDeduction([FromBody] DailyTimeRecordViewModel model)
        {
            return Ok(await _undertimeRepository.GetUndertimeDeduction(model));
        }

        [HttpPost]
        [Route("GetUndertimeDeductionByHeaderId")]
        public async Task<IActionResult> GetUndertimeDeductionByHeaderId([FromBody] UndertimeHeaderViewModel model)
        {
            return Ok(await _undertimeRepository.GetUndertimeDeductionByHeaderId(model));
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
            // return Ok(await _wfhRepository.SaveWFHTimeIn(model));
            var result = await _wfhRepository.SaveWFHTimeIn(model);
            return StatusCode(result.statuscode, result.message);
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
            var result = await _wfhRepository.SaveWFHApplication(param);
            return StatusCode(result.statuscode, result.message);
        }

        [HttpPost]
        [Route("CancelWFHApplication")]
        public async Task<IActionResult> CancelWFHApplication([FromBody] WFHHeaderViewModel model)
        {
            var result = await _wfhRepository.CancelWFHApplication(model);
            return StatusCode(result.statuscode, result.message);
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
        [Route("GetOvertimeSummaryAsync")]
        public async Task<IActionResult> GetOvertimeSummaryAsync([FromBody] OvertimePayReport model)
        {
            return Ok(await _overtimeRepository.GetOvertimeSummaryAsync(model));
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
            //return Ok(await _overtimeRepository.SaveOvertime(param));
            var result = await _overtimeRepository.SaveOvertime(param);
            return StatusCode(result.statuscode, result.message);
        }

        [HttpPost]
        [Route("CheckOvertimeDate")]
        public async Task<IActionResult> CheckOvertimeDate([FromBody] OvertimeEntryDto param)
        {
            return Ok(await _overtimeRepository.CheckOvertimeDate(param));
        }

        [HttpPost]
        [Route("GetAllLeaveReport")]
        public async Task<IActionResult> GetAllLeaveReport([FromBody] LeaveViewModel model)
        {
            return Ok(await _leaveRepository.GetAllLeaveReport());
        }

        [HttpPost]
        [Route("CancelLeave")]
        public async Task<IActionResult> CancelLeave([FromBody] LeaveRequestHeaderViewModel model)
        {
            var result = await _leaveRepository.CancelLeave(model);
            return StatusCode(result.statuscode, result.message);
        }


        [HttpPost]
        [Route("GetAllLate")]
        public async Task<IActionResult> GetAllLate(DailyTimeRecordViewModel model)
        {
            var result = await _lateRepository.GetAllLate(model);
            return Ok(result);
        }


        [HttpPost]
        [Route("GetLateById")]
        public async Task<IActionResult> GetLateById([FromBody] DailyTimeRecordViewModel model)
        {
            return Ok(await _lateRepository.GetLateById(model));
        }

        [HttpPost]
        [Route("SaveLate")]
        public async Task<IActionResult> SaveLate([FromBody] LateHeaderDeductionViewModel model)
        {          
            var result = await _lateRepository.SaveLate(model);
            return StatusCode(result.statuscode, result.message);
        }


        [HttpPost]
        [Route("GetLateDeduction")]
        public async Task<IActionResult> GetLateDeduction([FromBody] DailyTimeRecordViewModel model)
        {
            return Ok(await _lateRepository.GetLateDeduction(model));
        }


        [HttpPost]
        [Route("GetLateDeductionByHeaderId")]
        public async Task<IActionResult> GetLateDeductionByHeaderId([FromBody] LateHeaderViewModel model)
        {
            return Ok(await _lateRepository.GetLateDeductionByHeaderId(model));
        }


        [HttpPost]
        [Route("GetAllDTRByDate")] //Job Trigger. Everyday Email Attendance Confirmation
        public async Task<IActionResult> GetAllDTRByDate(DailyTimeRecordViewModel model)
        {
            var result = await _dtrRepository.GetAllDTRByDate(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("SaveLeaveManagement")]
        public async Task<IActionResult> SaveLeaveManagement([FromBody] LeaveFormViewModel model)
        {            
            var result = await _leaveRepository.SaveLeaveManagement(model);
            return StatusCode(result.statuscode, result.message);
        }

        [HttpPost]
        [Route("GetAllLeaveMangement")]
        public async Task<IActionResult> GetAllLeaveMangement([FromBody] LeaveViewModel model)
        {
            return Ok(await _leaveRepository.GetAllLeaveMangement(model));
        }


        [HttpPost]
        [Route("GetAllLeaveMangementByEmpId")]
        public async Task<IActionResult> GetAllLeaveMangementByEmpId([FromBody] LeaveViewModel model)
        {
            return Ok(await _leaveRepository.GetAllLeaveMangementByEmpId(model));
        }

        [HttpPost]
        [Route("GetAllLeaveReportForProbitionaryContractual")]
        public async Task<IActionResult> GetAllLeaveReportForProbitionaryContractual([FromBody] DailyTimeRecordViewModel param)
        {
            return Ok(await _leaveRepository.GetAllLeaveReportForProbitionaryContractual(param));
        }
       
    }
}
