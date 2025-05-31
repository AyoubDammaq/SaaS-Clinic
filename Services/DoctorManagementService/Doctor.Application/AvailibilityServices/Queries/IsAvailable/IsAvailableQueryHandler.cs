using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Queries.IsAvailable
{
    public class IsAvailableQueryHandler : IRequestHandler<IsAvailableQuery, bool>
    {
        private readonly IDisponibiliteRepository _disponibiliteRepository;
        public IsAvailableQueryHandler(IDisponibiliteRepository disponibiliteRepository) {
            _disponibiliteRepository = disponibiliteRepository;
        }

        public async Task<bool> Handle(IsAvailableQuery request, CancellationToken cancellationToken)
        {
            if (request.medecinId == Guid.Empty)
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.", nameof(request.medecinId));
            return await _disponibiliteRepository.EstDisponibleAsync(request.medecinId, request.dateTime);
        }
    }
}
