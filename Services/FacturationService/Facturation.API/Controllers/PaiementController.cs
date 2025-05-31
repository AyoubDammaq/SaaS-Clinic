using Facturation.Application.PaiementService.Commands.ImprimerRecuDePaiement;
using Facturation.Application.PaiementService.Commands.PayerFacture;
using Facturation.Application.PaiementService.Queries.GetPaiementByFactureId;
using Facturation.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Facturation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaiementController : Controller
    {
        private readonly IMediator _mediator;

        public PaiementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("payer/{factureId}")]
        public async Task<IActionResult> PayerFacture(Guid factureId, [FromBody] ModePaiement moyenPaiement)
        {
            if (factureId == Guid.Empty)
                return BadRequest("Identifiant de facture invalide.");

            if (!Enum.IsDefined(typeof(ModePaiement), moyenPaiement))
                return BadRequest("Mode de paiement invalide.");

            try
            {
                var success = await _mediator.Send(new PayerFactureCommand(factureId, moyenPaiement));
                if (!success)
                    return BadRequest("Paiement échoué : facture introuvable ou déjà payée.");

                return Ok("Paiement effectué avec succès.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Une erreur interne est survenue lors du paiement.");
            }
        }


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
