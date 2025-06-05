using ConsultationManagementService.Application.DTOs;
using MediatR;

namespace ConsultationManagementService.Application.Commands.UpdateConsultation
{
    public record UpdateConsultationCommand(ConsultationDTO consultation) : IRequest;
}
