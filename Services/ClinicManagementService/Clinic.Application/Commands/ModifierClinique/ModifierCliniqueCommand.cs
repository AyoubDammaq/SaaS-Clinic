using Clinic.Domain.Entities;
using MediatR;

namespace Clinic.Application.Commands.ModifierClinique
{
    public record ModifierCliniqueCommand(Guid Id, Clinique Clinique) : IRequest<Clinique>;
}
