using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Queries.ObtenirDisponibilitesDansIntervalle
{
    public class ObtenirDisponibilitesDansIntervalleQueryHandler : IRequestHandler<ObtenirDisponibilitesDansIntervalleQuery, List<Disponibilite>>
    {
        private readonly IDisponibiliteRepository _disponibiliteRepository;
        public ObtenirDisponibilitesDansIntervalleQueryHandler(IDisponibiliteRepository disponibiliteRepository)
        {
            _disponibiliteRepository = disponibiliteRepository;
        }

        public async Task<List<Disponibilite>> Handle(ObtenirDisponibilitesDansIntervalleQuery request, CancellationToken cancellationToken)
        {
            if (request.medecinId == Guid.Empty)
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.", nameof(request.medecinId));
            if (request.start >= request.end)
                throw new ArgumentException("La date de début doit être inférieure à la date de fin.");
            var disponibilites = await _disponibiliteRepository.ObtenirDisponibilitesDansIntervalleAsync(request.medecinId, request.start, request.end);

            return disponibilites ?? new List<Disponibilite>();
        }
    }
}
