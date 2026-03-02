using Application.Abstraction;
using Domain.DbTables;
using Microsoft.AspNetCore.Mvc;

namespace Gestionare_Bunuri_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        /// <summary>
        /// Descarcă un document după ID
        /// </summary>
        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadDocument(int id)
        {
            var result = await _documentService.DownloadDocumentAsync(id);
            if (result == null)
                return NotFound();

            return File(result.Value.fileBytes, result.Value.contentType, result.Value.fileName);
        }

        /// <summary>
        /// Șterge un document după ID
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var deleted = await _documentService.DeleteDocumentAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}
