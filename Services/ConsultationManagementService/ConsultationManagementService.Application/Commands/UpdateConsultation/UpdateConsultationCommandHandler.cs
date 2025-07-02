using AutoMapper;
using ConsultationManagementService.Application.DTOs;
using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Repositories;
using MediatR;

namespace ConsultationManagementService.Application.Commands.UpdateConsultation
{
    public class UpdateConsultationCommandHandler : IRequestHandler<UpdateConsultationCommand>
    {
        private readonly IConsultationRepository _consultationRepository;
        private readonly IMapper _mapper;

        public UpdateConsultationCommandHandler(IConsultationRepository consultationRepository, IMapper mapper)
        {
            _consultationRepository = consultationRepository;
            _mapper = mapper;
        }

        public async Task Handle(UpdateConsultationCommand request, CancellationToken cancellationToken)
        {
            if (request.consultation == null)
            {
                throw new ArgumentNullException(nameof(request.consultation), "Les données de la consultation ne peuvent pas être nulles.");
            }
            if (request.consultation.Id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant de la consultation ne peut pas être vide.", nameof(request.consultation));
            }

            ValidateConsultationData(request.consultation);

            // Utilisation du mapper pour transformer le DTO en entité du domaine
            var consultationEntity = _mapper.Map<Consultation>(request.consultation);

            consultationEntity.UpdateConsultationEvent();

            await _consultationRepository.UpdateConsultationAsync(consultationEntity);
        }

        private void ValidateConsultationData(ConsultationDTO consultation)
        {
            if (consultation.PatientId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du patient ne peut pas être vide.");
            }
            if (consultation.MedecinId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du médecin ne peut pas être vide.");
            }
            if (consultation.DateConsultation == default)
            {
                throw new ArgumentException("La date de consultation doit être spécifiée.");
            }
            if (string.IsNullOrWhiteSpace(consultation.Diagnostic))
            {
                throw new ArgumentException("Le diagnostic ne peut pas être vide.");
            }
        }
    }
}
