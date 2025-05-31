using MediatR;
using RDV.Application.DTOs;
using RDV.Domain.Interfaces;

namespace RDV.Application.Queries.GetAllRendezVous
{
    public class GetAllRendezVousQueryHandler : IRequestHandler<GetAllRendezVousQuery, IEnumerable<RendezVousDTO>>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        public GetAllRendezVousQueryHandler(IRendezVousRepository repository)
        {
            _rendezVousRepository = repository;
        }
        public async Task<IEnumerable<RendezVousDTO>> Handle(GetAllRendezVousQuery request, CancellationToken cancellationToken)
        {
            var rendezVousList = await _rendezVousRepository.GetAllRendezVousAsync();
            var dtoList = rendezVousList.Select(r => new RendezVousDTO
            {
                Id = r.Id,
                PatientId = r.PatientId,
                MedecinId = r.MedecinId,
                DateHeure = r.DateHeure,
                Statut = r.Statut,
                Commentaire = r.Commentaire ?? string.Empty
            });

            return dtoList;
        }
    }
}
