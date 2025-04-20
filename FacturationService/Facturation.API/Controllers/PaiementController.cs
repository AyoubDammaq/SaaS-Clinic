using Facturation.Application.Interfaces;
using Facturation.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Facturation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaiementController : Controller
    {
        private readonly IPaiementService _paiementService;

        public PaiementController(IPaiementService paiementService)
        {
            _paiementService = paiementService;
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
                var success = await _paiementService.PayerFactureAsync(factureId, moyenPaiement);

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
                var paiement = await _paiementService.GetPaiementByFactureIdAsync(factureId);

                if (paiement == null)
                    return NotFound("Aucun paiement trouvé pour cette facture.");

                var pdfBytes = await _paiementService.ImprimerRecuDePaiement(paiement);

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
