using Doctor.Domain.Entities;
using Doctor.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Domain.Interfaces
{
    public interface IMedecinRepository
    {
        Task<Medecin> GetByIdAsync(Guid id);
        Task<List<Medecin>> GetAllAsync();
        Task AddAsync(Medecin medecin);
        Task UpdateAsync(Medecin medecin);
        Task DeleteAsync(Guid id);
        Task<List<Medecin>> FilterBySpecialiteAsync(string specialite);
        Task<List<Medecin>> FilterByNameOrPrenomAsync(string name, string prenom);
        Task<List<Medecin>> GetMedecinByCliniqueIdAsync(Guid cliniqueId);
        Task AttribuerMedecinAUneCliniqueAsync(Guid medecinId, Guid cliniqueId);
        Task DesabonnerMedecinDeCliniqueAsync(Guid medecinId);


        Task<IEnumerable<StatistiqueMedecin>> GetNombreMedecinBySpecialiteAsync();
        Task<IEnumerable<StatistiqueMedecin>> GetNombreMedecinByCliniqueAsync();
        Task<IEnumerable<StatistiqueMedecin>> GetNombreMedecinBySpecialiteDansUneCliniqueAsync(Guid cliniqueId);
        Task<IEnumerable<Guid>> GetMedecinsIdsByCliniqueId(Guid cliniqueId);

        Task<IEnumerable<ActiviteMedecin>> GetActivitesMedecinAsync(Guid medecinId);

    }
}
