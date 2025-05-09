using DCI.Data;
using DCI.Models.ViewModel;
using DCI.Repositories;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DCI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : Controller
    {
        private readonly DCIdbContext _dcIdbContext;
        IEmployeeRepository _employeeRepository;

        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        [HttpGet]
        [Route("GetAllEmployee")]
        public async Task<IActionResult> GetAllEmployee()
        {           
            var result = await _employeeRepository.GetAllEmployee();
            return Ok(result);
        }


        [HttpPost]
        [Route("GetEmployeeById")]
        public async Task<IActionResult> GetEmployeeById([FromBody] Form201ViewModel model)
        {     
            return Ok(await _employeeRepository.GetEmployeeById(model.EmployeeId));
        }

        [HttpPost]
        [Route("SaveEmployee")]
        public async Task<IActionResult> SaveEmployee([FromBody] Form201ViewModel model)
        {          
            var result = await _employeeRepository.Save(model);
            return StatusCode(result.statuscode, result.message);
        }

        [HttpPost]
        [Route("DeleteEmployee")]
        public async Task<IActionResult> DeleteEmployee([FromBody] Form201ViewModel model)
        {
            var result = await _employeeRepository.Delete(model);
            return StatusCode(result.statuscode, result.message);
        }

        [HttpPost]
        [Route("Update201Form")]
        public async Task<IActionResult> Update201Form([FromBody] Form201ViewModel model)
        {
            var result = await _employeeRepository.Update201Form(model);
            return StatusCode(result.statuscode, result.message);           
        }
    }
}
