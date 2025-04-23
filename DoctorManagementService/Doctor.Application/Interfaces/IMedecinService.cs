using Doctor.Application.DTOs;
using Doctor.Domain.Entities;

namespace Doctor.Application.Interfaces
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
        Task<IEnumerable<MedecinDto>> GetMedecinByClinique(Guid cliniqueId);
        Task AttribuerMedecinAUneClinique(Guid medecinId, Guid cliniqueId);
        Task DesabonnerMedecinDeClinique(Guid medecinId);

        Task<IEnumerable<StatistiqueMedecinDTO>> GetNombreMedecinBySpecialite();
        Task<IEnumerable<StatistiqueMedecinDTO>> GetNombreMedecinByClinique();
        Task<IEnumerable<StatistiqueMedecinDTO>> GetNombreMedecinBySpecialiteDansUneClinique(Guid cliniqueId);
        Task<IEnumerable<Guid>> GetMedecinsIdsByCliniqueId(Guid cliniqueId);

        Task<IEnumerable<ActiviteMedecinDTO>> GetActivitesMedecin(Guid medecinId);
    }
}
