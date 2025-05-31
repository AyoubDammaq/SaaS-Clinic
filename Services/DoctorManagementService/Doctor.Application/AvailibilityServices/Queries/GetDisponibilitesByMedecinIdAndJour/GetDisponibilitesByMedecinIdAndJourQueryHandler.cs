using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Queries.GetDisponibilitesByMedecinIdAndJour
{
    public class GetDisponibilitesByMedecinIdAndJourQueryHandler : IRequestHandler<GetDisponibilitesByMedecinIdAndJourQuery, List<Disponibilite>>
    {
        private readonly IDisponibiliteRepository _disponibiliteRepository;
        public GetDisponibilitesByMedecinIdAndJourQueryHandler(IDisponibiliteRepository disponibiliteRepository)
        {
            _disponibiliteRepository = disponibiliteRepository;
        }
        public async Task<List<Disponibilite>> Handle(GetDisponibilitesByMedecinIdAndJourQuery request, CancellationToken cancellationToken)
        {
            if (request.medecinId == Guid.Empty)
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.", nameof(request.medecinId));
            var disponibilites = await _disponibiliteRepository.ObtenirDisponibilitesParJourAsync(request.medecinId, request.jour);

            return disponibilites ?? new List<Disponibilite>();
        }
    }
}
