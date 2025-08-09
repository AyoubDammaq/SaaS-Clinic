using MediatR;
using RDV.Application.DTOs;
using RDV.Domain.Enums;
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
            var now = DateTime.Now;

            foreach (var rdv in rendezVousList)
            {
                if (rdv.Statut == RDVstatus.EN_ATTENTE && rdv.DateHeure < now)
                {
                    rdv.Statut = RDVstatus.ANNULE;
                    rdv.Commentaire = "Annulé automatiquement : non confirmé avant la date prévue.";
                    await _rendezVousRepository.UpdateRendezVousAsync(rdv.Id, rdv);
                }
            }

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
