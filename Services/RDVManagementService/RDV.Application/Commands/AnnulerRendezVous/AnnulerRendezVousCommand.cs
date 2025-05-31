using MediatR;

namespace RDV.Application.Commands.AnnulerRendezVous
{
    public record AnnulerRendezVousCommand(Guid id) : IRequest<bool>;
}
