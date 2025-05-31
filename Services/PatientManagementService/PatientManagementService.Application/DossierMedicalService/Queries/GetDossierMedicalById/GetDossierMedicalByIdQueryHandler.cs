using MediatR;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.DossierMedicalService.Queries.GetDossierMedicalById
{
    public class GetDossierMedicalByIdQueryHandler : IRequestHandler<GetDossierMedicalByIdQuery, DossierMedical>
    {
        private readonly IDossierMedicalRepository _dossierMedicalRepository;

        public GetDossierMedicalByIdQueryHandler(IDossierMedicalRepository repository)
        {
            _dossierMedicalRepository = repository;
        }

        public async Task<DossierMedical> Handle(GetDossierMedicalByIdQuery request, CancellationToken cancellationToken)
        {
            var dossierMedical = await _dossierMedicalRepository.GetDossierMedicalByIdAsync(request.Id) ?? throw new Exception("Dossier médical not found");
            return dossierMedical;
        }
    }
}
