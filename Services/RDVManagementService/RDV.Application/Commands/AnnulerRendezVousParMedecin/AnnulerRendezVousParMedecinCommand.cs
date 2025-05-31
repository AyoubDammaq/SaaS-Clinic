using MediatR;

namespace RDV.Application.Commands.AnnulerRendezVousParMedecin
{
    public record AnnulerRendezVousParMedecinCommand(Guid rendezVousId, string justification) : IRequest;
}
