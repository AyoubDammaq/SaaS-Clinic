using Doctor.Application.DTOs;

namespace Doctor.Application.Interfaces
{
    public interface IRendezVousHttpClient
    {
        Task<List<RendezVousDTO>> GetRendezVousParMedecinEtDate(Guid medecinId, DateTime date);
    }
}
