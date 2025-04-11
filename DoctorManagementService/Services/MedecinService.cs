using DoctorManagementService.DTOs;
using DoctorManagementService.Models;
using DoctorManagementService.Repositories;

namespace DoctorManagementService.Services
{
    public class MedecinService : IMedecinService
    {
        private readonly IMedecinRepository _medecinRepository;

        public MedecinService(IMedecinRepository medecinRepository)
        {
            _medecinRepository = medecinRepository;
        }

        public async Task AddDoctor(Medecin medecin)
        {
            await _medecinRepository.AddAsync(medecin);
        }

        public async Task<MedecinDto> GetDoctorById(Guid id)
        {
            var medecin = await _medecinRepository.GetByIdAsync(id);
            if (medecin == null)
            {
                return null;
            }

            return new MedecinDto
            {
                Prenom = medecin.Prenom,
                Nom = medecin.Nom,
                Specialite = medecin.Specialite,
                CliniqueId = medecin.CliniqueId,
                Email = medecin.Email,
                Telephone = medecin.Telephone,
                PhotoUrl = medecin.PhotoUrl,
                Disponibilites = (List<Disponibilite>)medecin.Disponibilites
            };
        }

        public async Task<IEnumerable<MedecinDto>> GetAllDoctors()
        {
            var medecins = await _medecinRepository.GetAllAsync();
            return medecins.Select(m => new MedecinDto
            {
                Prenom = m.Prenom,
                Nom = m.Nom,
                Specialite = m.Specialite,
                CliniqueId = m.CliniqueId,
                Email = m.Email,
                Telephone = m.Telephone,
                PhotoUrl = m.PhotoUrl,
                Disponibilites = (List<Disponibilite>)m.Disponibilites
            });
        }

        public async Task UpdateDoctor(Guid id, MedecinDto medecinDto)
        {
            var medecin = await _medecinRepository.GetByIdAsync(id);
            if (medecin == null)
            {
                throw new Exception("Medecin not found");
            }

            medecin.Prenom = medecinDto.Prenom;
            medecin.Nom = medecinDto.Nom;
            medecin.Specialite = medecinDto.Specialite;
            medecin.CliniqueId = medecinDto.CliniqueId ?? Guid.Empty;
            medecin.Email = medecinDto.Email;
            medecin.Telephone = medecinDto.Telephone;
            medecin.PhotoUrl = medecinDto.PhotoUrl;

            await _medecinRepository.UpdateAsync(medecin);
        }

        public async Task DeleteDoctor(Guid id)
        {
            await _medecinRepository.DeleteAsync(id);
        }
        public async Task<IEnumerable<MedecinDto>> FilterDoctorsBySpecialite(string specialite)
        {
            var medecins = await _medecinRepository.FilterBySpecialiteAsync(specialite);
            return medecins.Select(m => new MedecinDto
            {
                Prenom = m.Prenom,
                Nom = m.Nom,
                Specialite = m.Specialite,
                CliniqueId = m.CliniqueId,
                Email = m.Email,
                Telephone = m.Telephone,
                PhotoUrl = m.PhotoUrl
            });
        }

        public async Task<IEnumerable<MedecinDto>> FilterDoctorsByName(string name, string prenom)
        {
            var medecins = await _medecinRepository.FilterByNameOrPrenomAsync(name, prenom);
            return medecins.Select(m => new MedecinDto
            {
                Prenom = m.Prenom,
                Nom = m.Nom,
                Specialite = m.Specialite,
                CliniqueId = m.CliniqueId,
                Email = m.Email,
                Telephone = m.Telephone,
                PhotoUrl = m.PhotoUrl
            });
        }

    }
}
