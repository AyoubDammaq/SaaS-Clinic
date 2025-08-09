using MediatR;
using PatientManagementService.Domain.Interfaces;

namespace PatientManagementService.Application.PatientService.Queries.GetNombreDeNouveauxPatientsParMois
{
    public class GetNombreDeNouveauxPatientsParMoisQueryHandler : IRequestHandler<GetNombreDeNouveauxPatientsParMoisQuery, Dictionary<string, int>>
    {
        private readonly IPatientRepository _patientRepository;
        public GetNombreDeNouveauxPatientsParMoisQueryHandler(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }
        public async Task<Dictionary<string, int>> Handle(GetNombreDeNouveauxPatientsParMoisQuery request, CancellationToken cancellationToken)
        {
            return await _patientRepository.GetNombreDeNouveauxPatientsParMoisAsync(request.Date);
        }
    }
}
