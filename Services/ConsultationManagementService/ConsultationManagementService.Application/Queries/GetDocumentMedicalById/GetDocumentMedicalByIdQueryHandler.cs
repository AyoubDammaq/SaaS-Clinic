using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Repositories;
using MediatR;

namespace ConsultationManagementService.Application.Queries.GetDocumentMedicalById
{
    public class GetDocumentMedicalByIdQueryHandler : IRequestHandler<GetDocumentMedicalByIdQuery, DocumentMedical?>
    {
        private readonly IConsultationRepository _consultationRepository;
        public GetDocumentMedicalByIdQueryHandler(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository;
        }
        public async Task<DocumentMedical?> Handle(GetDocumentMedicalByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du document médical ne peut pas être vide.", nameof(request.id));
            }
            return await _consultationRepository.GetDocumentMedicalByIdAsync(request.id);
        }
    }
}
