using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Queries.GetDisponibilites
{
    public class GetDisponibilitesQueryHandler : IRequestHandler<GetDisponibilitesQuery, List<Disponibilite>>
    {
        private readonly IDisponibiliteRepository _disponibiliteRepository;
        public GetDisponibilitesQueryHandler(IDisponibiliteRepository disponibiliteRepository)
        {
            _disponibiliteRepository = disponibiliteRepository;
        }
        public async Task<List<Disponibilite>> Handle(GetDisponibilitesQuery request, CancellationToken cancellationToken)
        {
            var disponibilites = await _disponibiliteRepository.ObtenirToutesDisponibilitesAsync();
            return disponibilites ?? new List<Disponibilite>();
        }
    }
}
