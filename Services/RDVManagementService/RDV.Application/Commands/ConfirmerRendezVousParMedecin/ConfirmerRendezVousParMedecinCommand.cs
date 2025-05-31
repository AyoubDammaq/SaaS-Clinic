using MediatR;

namespace RDV.Application.Commands.ConfirmerRendezVousParMedecin
{
    public record ConfirmerRendezVousParMedecinCommand(Guid rendezVousId) : IRequest;
}
