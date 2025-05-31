using ConsultationManagementService.Repositories;
using MediatR;

namespace Consultation.Application.Commands.DeleteConsultation
{
    public class DeleteConsultationCommandHandler : IRequestHandler<DeleteConsultationCommand, bool>
    {
        private readonly IConsultationRepository _consultationRepository;
        public DeleteConsultationCommandHandler(IConsultationRepository consultationRepository)
        {
            _consultationRepository = consultationRepository;
        }
        public async Task<bool> Handle(DeleteConsultationCommand request, CancellationToken cancellationToken)
        {
            if (request.id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant de la consultation ne peut pas être vide.", nameof(request.id));
            }
            return await _consultationRepository.DeleteConsultationAsync(request.id);
        }
    }
}
