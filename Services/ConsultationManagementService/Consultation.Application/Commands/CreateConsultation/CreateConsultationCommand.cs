using ConsultationManagementService.DTOs;
using MediatR;

namespace Consultation.Application.Commands.CreateConsultation
{
    public record CreateConsultationCommand(ConsultationDTO consultation) : IRequest; 
}
