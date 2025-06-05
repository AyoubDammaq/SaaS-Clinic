using Clinic.Domain.Entities;
using MediatR;

namespace Clinic.Application.Commands.ModifierClinique
{
    public record UpdateCliniqueCommand(Guid Id, Clinique Clinique) : IRequest<Clinique>;
}
