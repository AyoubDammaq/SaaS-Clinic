using Facturation.Domain.Entities;
using Facturation.Domain.Enums;
using Facturation.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Facturation.Application.FactureService.Commands.AddFacture
{
    public class AddFactureCommandHandler : IRequestHandler<AddFactureCommand>
    {
        private readonly IFactureRepository _factureRepository;
        private readonly ILogger<AddFactureCommandHandler> _logger;
        public AddFactureCommandHandler(IFactureRepository factureRepository, ILogger<AddFactureCommandHandler> logger)
        {
            _factureRepository = factureRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task Handle(AddFactureCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.request.MontantTotal <= 0)
                throw new ArgumentException("Le montant total de la facture doit être supérieur à zéro.", nameof(request.request.MontantTotal));

            var facture = new Facture
            {
                Id = Guid.NewGuid(),
                PatientId = request.request.PatientId,
                ConsultationId = request.request.ConsultationId,
                ClinicId = request.request.ClinicId,
                DateEmission = DateTime.UtcNow,
                MontantTotal = request.request.MontantTotal,
                Status = FactureStatus.IMPAYEE
            };

            facture.CreateFactureEvent();

            _logger.LogInformation("Création facture {FactureId} avec statut {Status}", facture.Id, facture.Status);
            await _factureRepository.AddFactureAsync(facture);
        }
    }
}
