using ConsultationManagementService.DTOs;
using MediatR;

namespace Consultation.Application.Commands.UpdateConsultation
{
    public record UpdateConsultationCommand(ConsultationDTO consultation) : IRequest;
}
