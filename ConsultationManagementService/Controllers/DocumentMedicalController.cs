using ConsultationManagementService.Data;
using ConsultationManagementService.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConsultationManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentMedicalController : ControllerBase
    {
        private readonly ConsultationDbContext _context;

        public DocumentMedicalController(ConsultationDbContext context)
        {
            _context = context;
        }

        // POST: api/document
        [HttpPost]
        public async Task<ActionResult<DocumentMedical>> UploadDocument(DocumentMedical document)
        {
            _context.DocumentsMedicaux.Add(document);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(UploadDocument), new { id = document.Id }, document);
        }
    }
}
