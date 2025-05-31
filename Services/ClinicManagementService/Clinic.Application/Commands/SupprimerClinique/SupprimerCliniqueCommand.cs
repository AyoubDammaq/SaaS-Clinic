using MediatR;

namespace Clinic.Application.Commands.SupprimerClinique
{
    public record SupprimerCliniqueCommand(Guid Id) : IRequest<bool>;
}
