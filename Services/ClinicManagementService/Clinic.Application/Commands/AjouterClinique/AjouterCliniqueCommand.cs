using Clinic.Application.DTOs;
using Clinic.Domain.Entities;
using MediatR;

namespace Clinic.Application.Commands.AjouterClinique
{
    public record AjouterCliniqueCommand(CliniqueDto CliniqueDto) : IRequest<Clinique>;
}
