using MediatR;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.DossierMedicalService.Queries.GetDossierMedicalByPatientId
{
    public class GetDossierMedicalByPatientIdQueryHandler : IRequestHandler<GetDossierMedicalByPatientIdQuery, DossierMedical>
    {
        private readonly IDossierMedicalRepository _dossierMedicalRepository;
        public GetDossierMedicalByPatientIdQueryHandler(IDossierMedicalRepository repository)
        {
            _dossierMedicalRepository = repository;
        }
        public async Task<DossierMedical> Handle(GetDossierMedicalByPatientIdQuery request, CancellationToken cancellationToken)
        {
            var dossierMedical = await _dossierMedicalRepository.GetDossierMedicalByPatientIdAsync(request.patientId)
                ?? throw new Exception("Dossier médical not found for the specified patient");
            return dossierMedical;
        }
    }
}
