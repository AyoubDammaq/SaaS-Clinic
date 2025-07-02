using MediatR;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.DossierMedicalService.Queries.GetDocumentById
{
    public class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, Document>
    {
        private readonly IDossierMedicalRepository _dossierMedicalRepository;
        public GetDocumentByIdQueryHandler(IDossierMedicalRepository dossierMedicalRepository)
        {
            _dossierMedicalRepository = dossierMedicalRepository;
        }
        public async Task<Document> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
        {
            var document = await _dossierMedicalRepository.GetDocumentByIdAsync(request.documentId);
            if (document == null)
                throw new Exception("Document not found");

            return document;
        }
    }
}
