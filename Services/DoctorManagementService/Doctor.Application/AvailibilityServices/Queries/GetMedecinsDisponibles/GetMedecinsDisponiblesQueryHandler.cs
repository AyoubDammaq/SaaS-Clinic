using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Queries.GetMedecinsDisponibles
{
    public class GetMedecinsDisponiblesQueryHandler : IRequestHandler<GetMedecinsDisponiblesQuery, List<Medecin>>
    {
        private readonly IDisponibiliteRepository _disponibiliteRepository;
        public GetMedecinsDisponiblesQueryHandler(IDisponibiliteRepository disponibiliteRepository)
        {
            _disponibiliteRepository = disponibiliteRepository;
        }
        public async Task<List<Medecin>> Handle(GetMedecinsDisponiblesQuery request, CancellationToken cancellationToken)
        {
            if (request.date == default)
                throw new ArgumentException("La date ne peut pas être vide.", nameof(request.date));
            var medecins = await _disponibiliteRepository.ObtenirMedecinsDisponiblesAsync(request.date, request.heureDebut, request.heureFin);

            return medecins ?? new List<Medecin>();
        }
    }
}
