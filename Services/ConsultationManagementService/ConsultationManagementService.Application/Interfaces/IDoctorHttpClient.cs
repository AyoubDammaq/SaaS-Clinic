using ConsultationManagementService.Application.DTOs;

namespace ConsultationManagementService.Application.Interfaces
{
    public interface IDoctorHttpClient
    {
        Task<GetMedecinDto> GetDoctorById(Guid id);
    }
}
