using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Queries.GetDisponibilitesByMedecinId
{
    public class GetDisponibilitesByMedecinIdQueryHandler : IRequestHandler<GetDisponibilitesByMedecinIdQuery, List<Disponibilite>>
    {
        private readonly IDisponibiliteRepository _disponibiliteRepository;
        public GetDisponibilitesByMedecinIdQueryHandler(IDisponibiliteRepository disponibiliteRepository)
        {
            _disponibiliteRepository = disponibiliteRepository;
        }
        public async Task<List<Disponibilite>> Handle(GetDisponibilitesByMedecinIdQuery request, CancellationToken cancellationToken)
        {
            if (request.medecinId == Guid.Empty)
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.", nameof(request.medecinId));

            var disponibilites = await _disponibiliteRepository.ObtenirDisponibilitesParMedecinIdAsync(request.medecinId);

            return disponibilites ?? new List<Disponibilite>();
        }
    }

}
