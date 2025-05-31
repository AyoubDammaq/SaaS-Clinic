using ConsultationManagementService.Models;
using MediatR;

namespace Consultation.Application.Queries.GetDocumentMedicalById
{
    public record GetDocumentMedicalByIdQuery(Guid id) : IRequest<DocumentMedical?>;
}
