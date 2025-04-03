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

        public async Task AddDoctor(MedecinDto medecinDto)
        {
            var medecin = new Medecin(
                Guid.NewGuid(),
                medecinDto.Prenom,
                medecinDto.Nom,
                medecinDto.Specialite,
                medecinDto.CliniqueId
            );

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
                CliniqueId = medecin.CliniqueId
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
                CliniqueId = m.CliniqueId
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
            medecin.CliniqueId = medecinDto.CliniqueId;

            await _medecinRepository.UpdateAsync(medecin);
        }

        public async Task DeleteDoctor(Guid id)
        {
            await _medecinRepository.DeleteAsync(id);
        }
        public async Task<IEnumerable<MedecinDto>> FilterDoctors(string specialite)
        {
            var medecins = await _medecinRepository.FilterAsync(specialite);
            return medecins.Select(m => new MedecinDto
            {
                Prenom = m.Prenom,
                Nom = m.Nom,
                Specialite = m.Specialite,
                CliniqueId = m.CliniqueId
            });
        }

    }
}
