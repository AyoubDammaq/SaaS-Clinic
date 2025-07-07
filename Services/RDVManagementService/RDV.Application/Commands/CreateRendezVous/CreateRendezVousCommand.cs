using MediatR;
using RDV.Application.DTOs;
using RDV.Domain.Entities;

namespace RDV.Application.Commands.CreateRendezVous
{
    public record CreateRendezVousCommand(CreateRendezVousDto rendezVous) : IRequest;
}
