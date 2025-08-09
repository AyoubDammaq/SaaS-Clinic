using Facturation.Application.DTOs;
using Facturation.Application.PaiementService.Commands.ImprimerRecuDePaiement;
using Facturation.Application.PaiementService.Commands.PayerFacture;
using Facturation.Application.PaiementService.Queries.GetDernierPaiementByPatientId;
using Facturation.Application.PaiementService.Queries.GetPaiementByFactureId;
using Facturation.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facturation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,ClinicAdmin,Doctor,Patient")]
    public class PaiementController : Controller
    {
        private readonly IMediator _mediator;

        public PaiementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "SuperAdmin, ClinicAdmin, Patient")]
        [HttpPost("payer/{factureId}")]
        public async Task<IActionResult> PayerFacture(Guid factureId, [FromBody] PaiementDto paiementDto)
        {
            if (factureId == Guid.Empty)
                return BadRequest(new { errorCode = "InvalidInvoiceId", message = "Identifiant de facture invalide." });

            if (!Enum.IsDefined(typeof(ModePaiement), paiementDto.MoyenPaiement))
                return BadRequest(new { errorCode = "InvalidPaymentMethod", message = "Mode de paiement invalide." });

            if (paiementDto.Montant <= 0)
                return BadRequest(new { errorCode = "InvalidAmount", message = "Montant de paiement invalide." });

            try
            {
                var success = await _mediator.Send(new PayerFactureCommand(factureId, paiementDto));
                if (!success)
                    return BadRequest(new { errorCode = "PaymentFailed", message = "Paiement échoué : facture introuvable ou déjà payée." });

                return Ok(new { message = "Paiement effectué avec succès." });
            }
            catch (InvalidOperationException ex)
            {
                var errorCode = ex.Message.Contains("carte") ? "InvalidCardDetails" : "InvalidPaymentAmount";
                return BadRequest(new { errorCode, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { errorCode = "InternalServerError", message = "Une erreur interne est survenue lors du paiement." });
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("recu/{factureId}")]
        public async Task<IActionResult> ImprimerRecuDePaiement(Guid factureId)
        {
            if (factureId == Guid.Empty)
                return BadRequest(new { errorCode = "InvalidInvoiceId", message = "L'identifiant de la facture est invalide." });

            try
            {
                var paiement = await _mediator.Send(new GetPaiementByFactureIdQuery(factureId));
                if (paiement == null)
                    return NotFound(new { errorCode = "PaymentNotFound", message = "Aucun paiement trouvé pour cette facture." });

                var pdfBytes = await _mediator.Send(new ImprimerRecuDePaiementCommand(paiement));
                var nomFichier = $"recu_paiement_{paiement.DatePaiement:yyyyMMdd}.pdf";
                return File(pdfBytes, "application/pdf", nomFichier);
            }
            catch (Exception)
            {
                return StatusCode(500, new { errorCode = "InternalServerError", message = "Une erreur est survenue lors de la génération du reçu." });
            }
        }


        [HttpGet("GetPaiementByFactureId/{factureId}")]
        public async Task<IActionResult> GetPaiementByFactureId(Guid factureId)
        {
            if (factureId == Guid.Empty)
                return BadRequest(new { errorCode = "InvalidInvoiceId", message = "Identifiant de facture invalide." });
            try
            {
                var paiement = await _mediator.Send(new GetPaiementByFactureIdQuery(factureId));
                if (paiement == null)
                    return NotFound(new { errorCode = "PaymentNotFound", message = "Aucun paiement trouvé pour cette facture." });
                return Ok(paiement);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorCode = "InternalServerError", message = ex.Message });
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("GetDernierPaiementByPatientId/{patientId}")]
        public async Task<IActionResult> GetDernierPaiementByPatientId(Guid patientId)
        {
            if (patientId == Guid.Empty)
                return BadRequest(new { errorCode = "InvalidPatientId", message = "Identifiant de patient invalide." });
            try
            {
                var paiement = await _mediator.Send(new GetDernierPaiementByPatientIdQuery(patientId));
                return Ok(paiement);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorCode = "InternalServerError", message = ex.Message });
            }
        }
    }
}