using ConsultationManagementService.Domain.Entities;
using MediatR;

namespace ConsultationManagementService.Application.Queries.GetDocumentMedicalById
{
    public record GetDocumentMedicalByIdQuery(Guid id) : IRequest<DocumentMedical?>;
}
