using MediatR;
using RDV.Domain.Entities;

namespace RDV.Application.Commands.CreateRendezVous
{
    public record CreateRendezVousCommand(RendezVous rendezVous) : IRequest;
}
