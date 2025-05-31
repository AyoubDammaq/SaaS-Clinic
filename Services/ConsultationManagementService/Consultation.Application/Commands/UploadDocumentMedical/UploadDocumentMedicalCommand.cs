using ConsultationManagementService.DTOs;
using MediatR;

namespace Consultation.Application.Commands.UploadDocumentMedical
{
    public record UploadDocumentMedicalCommand(DocumentMedicalDTO documentMedical) : IRequest;
}
