using DCI.Data;
using DCI.Models.ViewModel;
using DCI.Repositories;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

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

		[HttpGet]
		[Route("GetAllDocument")]
		public async Task<IActionResult> GetAllDocument()
		{
			return Ok(await _documentRepository.GetAllDocument());
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
		public async Task<IActionResult> SaveDocument(DocumentViewModel model)
		{
			var result = await _documentRepository.Save(model);
			return StatusCode(result.statuscode, result.message);
		}
	}
}
