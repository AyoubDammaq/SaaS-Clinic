using ConsultationManagementService.Application.DTOs;
using MediatR;

namespace ConsultationManagementService.Application.Commands.CreateConsultation
{
    public record CreateConsultationCommand(ConsultationDTO consultation) : IRequest; 
}
