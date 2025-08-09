using MediatR;
using RDV.Application.DTOs;

namespace RDV.Application.Queries.GetRendezVousHebdomadaireStatistiquesByDoctor
{
    public record GetRendezVousHebdomadaireStatistiquesByDoctorQuery(Guid MedecinId, DateTime DateDebut, DateTime DateFin)
        : IRequest<IEnumerable<RendezVousHebdoStatDto>>;
}
