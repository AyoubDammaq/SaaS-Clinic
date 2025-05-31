using MediatR;
using RDV.Application.DTOs;

namespace RDV.Application.Queries.GetStatistiques
{
    public record GetStatistiquesQuery(DateTime dateDebut, DateTime dateFin) : IRequest<IEnumerable<RendezVousStatDTO>>;
}
