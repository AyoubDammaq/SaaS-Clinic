using Doctor.Domain.Interfaces;
using MediatR;

namespace Doctor.Application.AvailibilityServices.Queries.GetTotalAvailableTime
{
    public class GetTotalAvailableTimeQueryHandler : IRequestHandler<GetTotalAvailableTimeQuery, TimeSpan>
    {
        private readonly IDisponibiliteRepository _disponibiliteRepository;
        public GetTotalAvailableTimeQueryHandler(IDisponibiliteRepository disponibiliteRepository)
        {
            _disponibiliteRepository = disponibiliteRepository;
        }
        public async Task<TimeSpan> Handle(GetTotalAvailableTimeQuery request, CancellationToken cancellationToken)
        {
            if (request.medecinId == Guid.Empty)
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.", nameof(request.medecinId));
            if (request.dateDebut >= request.dateFin)
                throw new ArgumentException("La date de début doit être inférieure à la date de fin.");
            return await _disponibiliteRepository.ObtenirTempsTotalDisponibleAsync(request.medecinId, request.dateDebut, request.dateFin);
        }
    }
}
