using MediatR;
using PatientManagementService.Domain.Entities;

namespace PatientManagementService.Application.DossierMedicalService.Queries.GetDocumentById
{
    public record GetDocumentByIdQuery(Guid documentId) : IRequest<Document>;
}
