using PatientManagementService.Data;

namespace PatientManagementService.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly PatientDbContext _context;
        public PatientRepository(PatientDbContext context)
        {
            _context = context;
        }
        // Implement methods for patient management here
    }
}
