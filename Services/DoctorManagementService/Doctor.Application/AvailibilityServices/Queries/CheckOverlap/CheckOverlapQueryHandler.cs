using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Queries.CheckOverlap
{
    public class CheckOverlapQueryHandler : IRequestHandler<CheckOverlapQuery, bool>
    {
        private readonly IDisponibiliteRepository _disponibiliteRepository;
        public CheckOverlapQueryHandler(IDisponibiliteRepository availabilityRepository)
        {
            _disponibiliteRepository = availabilityRepository;
        }
        public async Task<bool> Handle(CheckOverlapQuery request, CancellationToken cancellationToken)
        {
            if (request.dispo == null)
                throw new ArgumentNullException(nameof(request.dispo), "La disponibilité ne peut pas être null.");
            return await _disponibiliteRepository.VerifieChevauchementAsync(request.dispo);
        }
    }
}
