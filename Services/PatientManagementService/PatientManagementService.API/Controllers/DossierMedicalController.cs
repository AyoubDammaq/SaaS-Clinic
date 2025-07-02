using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PatientManagementService.Application.DTOs;
using MediatR;
using PatientManagementService.Application.DossierMedicalService.Queries.GetDossierMedicalByPatientId;
using PatientManagementService.Application.DossierMedicalService.Commands.AddDossierMedical;
using PatientManagementService.Application.DossierMedicalService.Commands.UpdateDossierMedical;
using PatientManagementService.Application.DossierMedicalService.Queries.GetDossierMedicalById;
using PatientManagementService.Application.DossierMedicalService.Commands.DeleteDossierMedical;
using PatientManagementService.Application.DossierMedicalService.Queries.GetAllDossiersMedicals;
using PatientManagementService.Application.DossierMedicalService.Commands.AttacherDocument;
using PatientManagementService.Application.DossierMedicalService.Queries.GetDocumentById;
using PatientManagementService.Application.DossierMedicalService.Commands.RemoveDocument;

namespace PatientManagementService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DossierMedicalController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DossierMedicalController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("{patientId}")]
        public async Task<IActionResult> GetDossierMedicalByPatientId(Guid patientId)
        {
            try
            {
                var dossierMedical = await _mediator.Send(new GetDossierMedicalByPatientIdQuery(patientId));

                return Ok(dossierMedical);
            }
            catch (Exception)
            {
                return StatusCode(500, "Une erreur interne est survenue.");
            }
        }

        [Authorize(Roles = "SuperAdmin,ClinicAdmin,Doctor")] // Removed parentheses, fixed comma spacing
        [HttpPost]
        public async Task<IActionResult> AddDossierMedical([FromBody] DossierMedicalDTO dossierMedical)
        {
            try
            {
                if (dossierMedical == null)
                    return BadRequest(new { success = false, message = "Données invalides pour le dossier médical." });

                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });

                dossierMedical.Id = Guid.NewGuid();
                await _mediator.Send(new AddDossierMedicalCommand(dossierMedical));

                return CreatedAtAction(nameof(GetDossierMedicalByPatientId), new { patientId = dossierMedical.PatientId }, new
                {
                    success = true,
                    message = "Dossier médical ajouté avec succès.",
                    data = dossierMedical
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur interne lors de l'ajout du dossier médical.",
                    error = ex.Message
                });
            }
        }

        [Authorize(Roles = "SuperAdmin,ClinicAdmin,Doctor")]
        [HttpPut]
        public async Task<IActionResult> UpdateDossierMedical([FromBody] DossierMedicalDTO dossierMedical)
        {
            try
            {
                if (dossierMedical == null)
                    return BadRequest(new { success = false, message = "Données invalides pour le dossier médical." });

                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });

                await _mediator.Send(new UpdateDossierMedicalCommand(dossierMedical));
                return Ok(new { success = true, message = "Dossier médical mis à jour avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur interne lors de la mise à jour.",
                    error = ex.Message
                });
            }
        }

        [Authorize(Roles = "SuperAdmin,ClinicAdmin,Doctor")]
        [HttpDelete("{dossierMedicalId}")]
        public async Task<IActionResult> DeleteDossierMedical(Guid dossierMedicalId)
        {
            try
            {
                var dossierMedical = await _mediator.Send(new GetDossierMedicalByIdQuery(dossierMedicalId));
                if (dossierMedical == null)
                    return NotFound(new { success = false, message = "Dossier médical introuvable." });

                await _mediator.Send(new DeleteDossierMedicalCommand(dossierMedicalId));
                return Ok(new { success = true, message = "Dossier médical supprimé avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur interne lors de la suppression.",
                    error = ex.Message
                });
            }
        }

        [Authorize(Roles = "SuperAdmin,ClinicAdmin,Doctor")]
        [HttpGet]
        public async Task<IActionResult> GetAllDossiersMedicals()
        {
            try
            {
                var dossiersMedicals = await _mediator.Send(new GetAllDossiersMedicalsQuery());
                return Ok(dossiersMedicals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur interne lors de la récupération.",
                    error = ex.Message
                });
            }
        }

        [Authorize(Roles = "SuperAdmin,ClinicAdmin,Doctor")]
        [HttpPost("{dossierId}/documents")]
        public async Task<IActionResult> AttacherDocument(Guid dossierId, [FromBody] CreateDocumentRequest document)
        {
            try
            {
                var dossierMedical = await _mediator.Send(new GetDossierMedicalByIdQuery(dossierId));
                if (dossierMedical == null)
                    return NotFound(new { success = false, message = "Dossier médical introuvable." });

                await _mediator.Send(new AttacherDocumentCommand(dossierId, document));

                return Ok(new { success = true, message = "Document attaché avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur interne lors de l'attachement du document.",
                    error = ex.Message
                });
            }
        }

        [Authorize(Roles = "SuperAdmin,ClinicAdmin,Doctor")]
        [HttpGet("documents/{dossierMedicalId}")]
        public async Task<IActionResult> GetDossierMedicalById(Guid dossierMedicalId)
        {
            try
            {
                var dossierMedical = await _mediator.Send(new GetDossierMedicalByIdQuery(dossierMedicalId));
                return Ok(new
                {
                    success = true,
                    message = "Dossier médical récupéré avec succès.",
                    data = dossierMedical
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur lors de la récupération du dossier médical.",
                    error = ex.Message
                });
            }
        }

        [Authorize(Roles = "SuperAdmin,ClinicAdmin,Doctor")]
        [HttpDelete("documents/{documentId}")]
        public async Task<IActionResult> RemoveDocument(Guid documentId)
        {
            try
            {
                var document = await _mediator.Send(new GetDocumentByIdQuery(documentId));
                if (document == null)
                    return NotFound(new { success = false, message = "Document introuvable." });
                await _mediator.Send(new RemoveDocumentCommand(documentId));
                return Ok(new { success = true, message = "Document supprimé avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur lors de la suppression du document.",
                    error = ex.Message
                });
            }
        }

    }
}
