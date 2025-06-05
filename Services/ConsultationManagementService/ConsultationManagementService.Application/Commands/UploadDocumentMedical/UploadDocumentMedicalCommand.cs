using ConsultationManagementService.Application.DTOs;
using MediatR;

namespace ConsultationManagementService.Application.Commands.UploadDocumentMedical
{
    public record UploadDocumentMedicalCommand(DocumentMedicalDTO documentMedical) : IRequest;
}
