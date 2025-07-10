using MediatR;
using RDV.Domain.Entities;
using RDV.Domain.Interfaces;

namespace RDV.Application.Queries.GetRendezVousParMedecinEtDate
{
    public class GetRendezVousParMedecinEtDateQueryHandler : IRequestHandler<GetRendezVousParMedecinEtDateQuery, IEnumerable<RendezVous>>
    {
        private readonly IRendezVousRepository _repository;

        public GetRendezVousParMedecinEtDateQueryHandler(IRendezVousRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<RendezVous>> Handle(GetRendezVousParMedecinEtDateQuery request, CancellationToken cancellationToken)
        {
            if (request.MedecinId == Guid.Empty)
                throw new ArgumentException("MedecinId est vide.");

            var date = request.Date.Date;

            return await _repository.GetRendezVousByMedecinAndDateAsync(request.MedecinId, date);
        }
    }
}
