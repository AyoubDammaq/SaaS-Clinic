using MediatR;
using RDV.Domain.Entities;

namespace RDV.Application.Queries.GetRendezVousById
{
    public record GetRendezVousByIdQuery(Guid id) : IRequest<RendezVous>;
}
