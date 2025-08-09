using AutoMapper;
using ConsultationManagementService.Application.DTOs;
using ConsultationManagementService.Application.Interfaces;
using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Domain.Enums;
using ConsultationManagementService.Repositories;
using MediatR;

namespace ConsultationManagementService.Application.Commands.CreateConsultation
{
    public class CreateConsultationCommandHandler : IRequestHandler<CreateConsultationCommand>
    {
        private readonly IConsultationRepository _consultationRepository;
        private readonly IDoctorHttpClient _doctorServiceClient;
        private readonly IMapper _mapper;

        public CreateConsultationCommandHandler(IConsultationRepository consultationRepository, IMapper mapper, IDoctorHttpClient doctorHttpClient)
        {
            _consultationRepository = consultationRepository;
            _doctorServiceClient = doctorHttpClient;
            _mapper = mapper;
        }
        public async Task Handle(CreateConsultationCommand request, CancellationToken cancellationToken)
        {
            if (request.consultation == null)
            {
                throw new ArgumentNullException(nameof(request.consultation), "Les données de la consultation ne peuvent pas être nulles.");
            }

            // Appel HTTP pour récupérer le médecin
            var medecin = await _doctorServiceClient.GetDoctorById(request.consultation.MedecinId);

            if (medecin == null || medecin.CliniqueId == null || medecin.CliniqueId == Guid.Empty)
                throw new InvalidOperationException($"Le médecin avec l'id {request.consultation.MedecinId} n'est pas assigné à une clinique.");

            // Injecter ClinicId récupéré
            request.consultation.ClinicId = medecin.CliniqueId.Value;

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
            if (consultation.ClinicId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant de la clinique ne peut pas être vide.");
            }
            if (!Enum.IsDefined(typeof(TypeConsultation), consultation.Type))
            {
                throw new ArgumentException("Type de consultation invalide.");
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
