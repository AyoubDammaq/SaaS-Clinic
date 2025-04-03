using DoctorManagementService.DTOs;

namespace DoctorManagementService.Services
{
    public interface IMedecinService
    {
        Task AddDoctor(MedecinDto medecinDto);
        Task<MedecinDto> GetDoctorById(Guid id);
        Task<IEnumerable<MedecinDto>> GetAllDoctors();
        Task UpdateDoctor(Guid id, MedecinDto medecinDto);
        Task DeleteDoctor(Guid id);
    }
}
