using DoctorManagementService.DTOs;
using DoctorManagementService.Models;
using System.Threading.Tasks;

namespace DoctorManagementService.Services
{
    public interface IMedecinService
    {
        Task AddDoctor(Medecin medecin);
        Task<MedecinDto> GetDoctorById(Guid id);
        Task<IEnumerable<MedecinDto>> GetAllDoctors();
        Task UpdateDoctor(Guid id, MedecinDto medecinDto);
        Task DeleteDoctor(Guid id);
        Task<IEnumerable<MedecinDto>> FilterDoctorsBySpecialite(string specialite);
        Task<IEnumerable<MedecinDto>> FilterDoctorsByName(string name, string prenom);

    }
}
