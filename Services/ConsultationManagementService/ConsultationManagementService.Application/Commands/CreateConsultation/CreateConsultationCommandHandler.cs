using AutoMapper;
using ConsultationManagementService.Application.DTOs;
using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Repositories;
using MediatR;

namespace ConsultationManagementService.Application.Commands.CreateConsultation
{
    public class CreateConsultationCommandHandler : IRequestHandler<CreateConsultationCommand>
    {
        private readonly IConsultationRepository _consultationRepository;
        private readonly IMapper _mapper;
        public CreateConsultationCommandHandler(IConsultationRepository consultationRepository, IMapper mapper)
        {
            _consultationRepository = consultationRepository;
            _mapper = mapper;
        }
        public async Task Handle(CreateConsultationCommand request, CancellationToken cancellationToken)
        {
            if (request.consultation == null)
            {
                throw new ArgumentNullException(nameof(request.consultation), "Les données de la consultation ne peuvent pas être nulles.");
            }

            ValidateConsultationData(request.consultation); // Validation des données

            // Transformation de ConsultationDTO en Consultation
            var consultationEntity = _mapper.Map<Consultation>(request.consultation);

            consultationEntity.CreateConsultationEvent(); // Ajout de l'événement de création

            await _consultationRepository.CreateConsultationAsync(consultationEntity);
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
