using MediatR;
using PatientManagementService.Domain.Entities;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.DossierMedicalService.Queries.GetAllDossiersMedicals
{
    public class GetAllDossiersMedicalsQueryHandler : IRequestHandler<GetAllDossiersMedicalsQuery, IEnumerable<DossierMedical>>
    {
        private readonly IDossierMedicalRepository _dossierMedicalRepository;

        public GetAllDossiersMedicalsQueryHandler(IDossierMedicalRepository repository)
        {
            _dossierMedicalRepository = repository;
        }

        public async Task<IEnumerable<DossierMedical>> Handle(GetAllDossiersMedicalsQuery request, CancellationToken cancellationToken)
        {
            return await _dossierMedicalRepository.GetAllDossiersMedicalsAsync();
        }
    }
}
