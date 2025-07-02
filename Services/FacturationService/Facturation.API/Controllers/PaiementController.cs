using Facturation.Application.DTOs;
using Facturation.Application.PaiementService.Commands.ImprimerRecuDePaiement;
using Facturation.Application.PaiementService.Commands.PayerFacture;
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

        [Authorize(Roles = "Patient")]
        [HttpPost("payer/{factureId}")]
        public async Task<IActionResult> PayerFacture(Guid factureId, [FromBody] PaiementDto paiementDto)
        {
            if (factureId == Guid.Empty)
                return BadRequest("Identifiant de facture invalide.");

            if (!Enum.IsDefined(typeof(ModePaiement), paiementDto.MoyenPaiement))
                return BadRequest("Mode de paiement invalide.");

            if (paiementDto.Montant <= 0)
                return BadRequest("Montant de paiement invalide.");

            try
            {
                var success = await _mediator.Send(new PayerFactureCommand(factureId, paiementDto.MoyenPaiement, paiementDto.Montant));
                if (!success)
                    return BadRequest("Paiement échoué : facture introuvable ou déjà payée.");

                return Ok("Paiement effectué avec succès.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (Exception)
            {
                return StatusCode(500, "Une erreur interne est survenue lors du paiement.");
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("recu/{factureId}")]
        public async Task<IActionResult> ImprimerRecuDePaiement(Guid factureId)
        {
            if (factureId == Guid.Empty)
                return BadRequest("L'identifiant de la facture est invalide.");

            try
            {
                var paiement = await _mediator.Send(new GetPaiementByFactureIdQuery(factureId));

                if (paiement == null)
                    return NotFound("Aucun paiement trouvé pour cette facture.");

                var pdfBytes = await _mediator.Send(new ImprimerRecuDePaiementCommand(paiement));

                var nomFichier = $"recu_paiement_{paiement.DatePaiement:yyyyMMdd}.pdf";

                return File(pdfBytes, "application/pdf", nomFichier);
            }
            catch (Exception)
            {
                return StatusCode(500, "Une erreur est survenue lors de la génération du reçu.");
            }
        }

    }
}
