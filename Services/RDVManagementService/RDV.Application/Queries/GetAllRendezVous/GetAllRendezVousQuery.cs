using MediatR;
using RDV.Application.DTOs;

namespace RDV.Application.Queries.GetAllRendezVous
{
    public record GetAllRendezVousQuery() : IRequest<IEnumerable<RendezVousDTO>>;
}
