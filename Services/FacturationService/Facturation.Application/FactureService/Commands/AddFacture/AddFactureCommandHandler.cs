using Facturation.Domain.Entities;
using Facturation.Domain.Enums;
using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.FactureService.Commands.AddFacture
{
    public class AddFactureCommandHandler : IRequestHandler<AddFactureCommand>
    {
        private readonly IFactureRepository _factureRepository;
        public AddFactureCommandHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository;
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
                Status = FactureStatus.EN_ATTENTE
            };

            await _factureRepository.AddFactureAsync(facture);
        }
    }
}
