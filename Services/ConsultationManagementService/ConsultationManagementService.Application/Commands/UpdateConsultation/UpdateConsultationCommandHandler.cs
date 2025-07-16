using AutoMapper;
using ConsultationManagementService.Application.DTOs;
using ConsultationManagementService.Application.Interfaces;
using ConsultationManagementService.Domain.Entities;
using ConsultationManagementService.Repositories;
using MediatR;

namespace ConsultationManagementService.Application.Commands.UpdateConsultation
{
    public class UpdateConsultationCommandHandler : IRequestHandler<UpdateConsultationCommand>
    {
        private readonly IConsultationRepository _consultationRepository;
        private readonly IDoctorHttpClient _doctorServiceClient;
        private readonly IMapper _mapper;

        public UpdateConsultationCommandHandler(
            IConsultationRepository consultationRepository,
            IMapper mapper,
            IDoctorHttpClient doctorHttpClient)  // injection
        {
            _consultationRepository = consultationRepository;
            _doctorServiceClient = doctorHttpClient;
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

            // Récupérer le médecin et injecter ClinicId
            var medecin = await _doctorServiceClient.GetDoctorById(request.consultation.MedecinId);
            if (medecin == null || medecin.CliniqueId == null || medecin.CliniqueId == Guid.Empty)
                throw new InvalidOperationException($"Le médecin avec l'id {request.consultation.MedecinId} n'est pas assigné à une clinique.");

            request.consultation.ClinicId = medecin.CliniqueId.Value;

            ValidateConsultationData(request.consultation);

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
            if (consultation.ClinicId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant de la clinique ne peut pas être vide.");
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
