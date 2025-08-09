using MediatR;
using RDV.Application.DTOs;

namespace RDV.Application.Queries.GetRendezVousHebdomadaireStatistiquesByClinic
{
    public record GetRendezVousHebdomadaireStatistiquesByClinicQuery(Guid CliniqueId, DateTime DateDebut, DateTime DateFin)
        : IRequest<IEnumerable<RendezVousHebdoStatDto>>;
}
