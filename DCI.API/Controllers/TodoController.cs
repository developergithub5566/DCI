using DCI.Data;
using DCI.Models.ViewModel;
using DCI.Repositories;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DCI.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]

	public class TodoController : Controller
	{
		
		ITodoRepository _todoRepository;


		public TodoController(ITodoRepository todoRepository)
		{
			_todoRepository = todoRepository;
		}

        //[HttpPost]
        //[Route("GetTodoByApproverId")]
        //public async Task<IActionResult> GetTodoByApproverId([FromBody] ApprovalHistoryViewModel model)
        //{		
        //	return Ok(await _todoRepository.GetTodoByApproverId(model));
        //}

        [HttpPost]
        [Route("ApprovalLeave")]
        public async Task<IActionResult> ApprovalLeave([FromBody] ApprovalHistoryViewModel model)
        {
             var result = await _todoRepository.ApprovalLeave(model);
            return StatusCode(result.statuscode, result.message);
        }

        [HttpPost]
        [Route("ApprovalDtr")]
        public async Task<IActionResult> ApprovalDtr([FromBody] ApprovalHistoryViewModel model)
        {
            var result = await _todoRepository.ApprovalDtr(model);
            return StatusCode(result.statuscode, result.message);
        }

        [HttpPost]
        [Route("ApprovalWFH")]
        public async Task<IActionResult> ApprovalWFH([FromBody] ApprovalHistoryViewModel model)
        {
            var result = await _todoRepository.ApprovalWFH(model);
            return StatusCode(result.statuscode, result.message);
        }
        //[HttpPost]
        //[Route("GetAllTodo")]
        //public async Task<IActionResult> GetAllTodo([FromBody] DocumentViewModel model)
        //{	
        //	var result = await _todoRepository.GetAllTodo(model);
        //	return Ok(result);
        //}

        [HttpPost]
        [Route("GetAllTodoLeave")]
        public async Task<IActionResult> GetAllTodoLeave([FromBody] LeaveViewModel model)
        {
            var result = await _todoRepository.GetAllTodoLeave(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetAllTodoDtr")]
        public async Task<IActionResult> GetAllTodoDtr([FromBody] DTRCorrectionViewModel model)
        {
            var result = await _todoRepository.GetAllTodoDtr(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetApprovalHistory")]
        public async Task<IActionResult> GetApprovalHistory([FromBody] ApprovalHistoryViewModel model)
        {
            var result = await _todoRepository.GetApprovalHistory(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetAllTodoOvertime")]
        public async Task<IActionResult> GetAllTodoOvertime([FromBody] OvertimeViewModel model)
        {
            var result = await _todoRepository.GetAllTodoOvertime(model);
            return Ok(result);
        }


        [HttpPost]
        [Route("GetAllWFH")]
        public async Task<IActionResult> GetAllWFH([FromBody] WFHHeaderViewModel model)
        {
            var result = await _todoRepository.GetAllWFH(model);
            return Ok(result);
        }

    }
}
