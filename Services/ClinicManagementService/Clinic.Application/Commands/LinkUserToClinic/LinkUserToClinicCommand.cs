using Clinic.Application.DTOs;
using MediatR;

namespace Clinic.Application.Commands.LinkUserToClinic
{
    public record LinkUserToClinicCommand(LinkDTO Link) : IRequest<bool>;
}
