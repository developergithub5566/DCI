using DCI.API.Service;
using DCI.Data;
using DCI.Models.ViewModel;
using DCI.Repositories;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DCI.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DocumentController : Controller
	{
		private readonly DCIdbContext _dcIdbContext;
		IDocumentRepository _documentRepository;

		public DocumentController(IDocumentRepository documentRepository)
        {
            this._documentRepository = documentRepository;			
		}

		//[HttpGet]
		//[Route("GetAllDocument")]
		//public async Task<IActionResult> GetAllDocument()
		//{
		//	return Ok(await _documentRepository.GetAllDocument());
		//}
		[HttpPost]
		[Route("GetAllDocument")]
		public async Task<IActionResult> GetAllDocument([FromBody] DocumentViewModel model)
		{
			return Ok(await _documentRepository.GetAllDocument(model));
		}

		[HttpPost]
		[Route("GetDocumentById")]
		public async Task<IActionResult> GetDocumentById([FromBody] DocumentViewModel model)
		{
			return Ok(await _documentRepository.GetDocumentById(model.DocId));
		}

		[HttpPost]
		[Route("SaveDocument")]
		//public async Task<IActionResult> SaveDocument([FromBody] DocumentViewModel model)
		public async Task<IActionResult> SaveDocument( DocumentViewModel model)
		{
			var result = await _documentRepository.Save(model);
			return StatusCode(result.statuscode, result.message);
		}


		[HttpPost]
		[Route("DeleteDocument")]
		public async Task<IActionResult> DeleteDocument([FromBody] DocumentViewModel model)
		{

			var result = await _documentRepository.Delete(model);
			return StatusCode(result.statuscode, result.message);
		}


		[HttpPost]
		[Route("ValidateToken")]
		public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenViewModel model)
		{
			try
			{
				var result = await _documentRepository.ValidateToken(model);
				//return StatusCode(result.statuscode, result.message);
				if (result.DocId > 0)
				{
					return Ok(await _documentRepository.ValidateToken(model));
				}
				return BadRequest(await _documentRepository.ValidateToken(model));
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
			return BadRequest();
		}

		[HttpPost]
		[Route("UploadFile")]
		public async Task<IActionResult> UploadFile(DocumentViewModel model)
		{
			var result = await _documentRepository.UploadFile(model);
			return StatusCode(result.statuscode, result.message);
		}

		[HttpGet]
		[Route("HomePage")]
		public async Task<IActionResult> HomePage()
		{			
			return Ok(await _documentRepository.HomePage());
		}

		[HttpPost]
		[Route("WorkflowByDocId")]
		public async Task<IActionResult> WorkflowByDocId([FromBody] DocumentViewModel model)
		{
			return Ok(await _documentRepository.Workflow(model));
		}
	}
}
