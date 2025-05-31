using MediatR;
using RDV.Domain.Entities;

namespace RDV.Application.Commands.UpdateRendezVous
{
    public record UpdateRendezVousCommand(Guid id, RendezVous rendezVous) : IRequest;
}
