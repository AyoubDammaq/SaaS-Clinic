using Clinic.Domain.Entities;
using MediatR;

namespace Clinic.Application.Queries.ObtenirCliniqueParId
{
    public record ObtenirCliniqueParIdQuery(Guid Id) : IRequest<Clinique>;
}
