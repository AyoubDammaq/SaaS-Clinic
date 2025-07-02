using AutoMapper;
using MediatR;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.DossierMedicalService.Commands.AddDossierMedical
{
    public class AddDossierMedicalCommandHandler : IRequestHandler<AddDossierMedicalCommand>
    {
        private readonly IDossierMedicalRepository _dossierMedicalRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;

        public AddDossierMedicalCommandHandler(IDossierMedicalRepository repository, IPatientRepository patientRepository, IMapper mapper)
        {
            _dossierMedicalRepository = repository;
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        public async Task Handle(AddDossierMedicalCommand request, CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(request.dossierMedical.PatientId)
                ?? throw new Exception("Patient not found");

            if (patient.DossierMedicalId != null)
            {
                throw new InvalidOperationException("Patient already has a dossier médical.");
            }

            var dossierMedicalEntity = _mapper.Map<DossierMedical>(request.dossierMedical);
            dossierMedicalEntity.DateCreation = DateTime.UtcNow;

            patient.DossierMedicalId = dossierMedicalEntity.Id;

            dossierMedicalEntity.CreerDossierMedicalEvent();
            patient.ModifierPatientEvent();

            await _dossierMedicalRepository.AddDossierMedicalAsync(dossierMedicalEntity);
            await _patientRepository.UpdatePatientAsync(patient);
        }
    }
}
