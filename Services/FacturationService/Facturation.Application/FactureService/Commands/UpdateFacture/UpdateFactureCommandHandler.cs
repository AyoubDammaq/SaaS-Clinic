using Facturation.Domain.Interfaces;
using MediatR;

namespace Facturation.Application.FactureService.Commands.UpdateFacture
{
    public class UpdateFactureCommandHandler : IRequestHandler<UpdateFactureCommand>
    {
        private readonly IFactureRepository _factureRepository;
        public UpdateFactureCommandHandler(IFactureRepository factureRepository)
        {
            _factureRepository = factureRepository ?? throw new ArgumentNullException(nameof(factureRepository));
        }
        public async Task Handle(UpdateFactureCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.request.Id == Guid.Empty)
                throw new ArgumentException("L'identifiant de la facture ne peut pas être vide.", nameof(request.request.Id));

            if (request.request.PatientId == Guid.Empty)
                throw new ArgumentException("L'identifiant du patient ne peut pas être vide.", nameof(request.request.PatientId));

            if (request.request.ConsultationId == Guid.Empty)
                throw new ArgumentException("L'identifiant de la consultation ne peut pas être vide.", nameof(request.request.ConsultationId));

            if (request.request.ClinicId == Guid.Empty)
                throw new ArgumentException("L'identifiant de la clinique ne peut pas être vide.", nameof(request.request.ClinicId));

            var existingFacture = await _factureRepository.GetFactureByIdAsync(request.request.Id);
            if (existingFacture == null)
                throw new KeyNotFoundException($"Aucune facture trouvée avec l'identifiant {request.request.PatientId}.");

            // Map UpdateFactureRequest to Facture entity directly
            existingFacture.PatientId = request.request.PatientId;
            existingFacture.ConsultationId = request.request.ConsultationId;
            existingFacture.ClinicId = request.request.ClinicId;
            existingFacture.MontantTotal = request.request.MontantTotal;

            await _factureRepository.UpdateFactureAsync(existingFacture);
        }
    }
}
