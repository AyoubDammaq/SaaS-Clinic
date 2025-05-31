using ConsultationManagementService.Repositories;
using MediatR;

namespace Consultation.Application.Queries.GetDocumentMedicalById
{
    public class GetDocumentMedicalByIdQueryHandler : IRequestHandler<GetDocumentMedicalByIdQuery, ConsultationManagementService.Models.DocumentMedical?>
    {
        private readonly IConsultationRepository _consultationRepository;
        public GetDocumentMedicalByIdQueryHandler(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository;
        }
        public async Task<ConsultationManagementService.Models.DocumentMedical?> Handle(GetDocumentMedicalByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du document médical ne peut pas être vide.", nameof(request.id));
            }
            return await _consultationRepository.GetDocumentMedicalByIdAsync(request.id);
        }
    }
}
