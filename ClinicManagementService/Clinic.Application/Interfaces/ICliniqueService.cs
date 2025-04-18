﻿using Clinic.Domain.Entities;

namespace Clinic.Application.Interfaces
{
    public interface ICliniqueService
    {
        Task<Clinique> AjouterCliniqueAsync(Clinique clinique);
        Task<Clinique> ModifierCliniqueAsync(Guid id, Clinique clinique);
        Task<bool> SupprimerCliniqueAsync(Guid id);
        Task<Clinique> ObtenirCliniqueParIdAsync(Guid id);
        Task<List<Clinique>> ListerCliniqueAsync();
        Task<IEnumerable<Clinique>> ListerCliniquesParNomAsync(string nom);
        Task<IEnumerable<Clinique>> ListerCliniquesParAdresseAsync(string adresse);
    }
}
