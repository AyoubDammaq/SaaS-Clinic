using Doctor.Application.DTOs;
using Doctor.Application.Interfaces;
using Doctor.Domain.Entities;
using Doctor.Domain.Interfaces;


namespace Doctor.Application.Services
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
            if (medecin == null)
            {
                throw new ArgumentNullException(nameof(medecin), "Le médecin ne peut pas être null.");
            }

            await _medecinRepository.AddAsync(medecin);
        }

        public async Task<MedecinDto> GetDoctorById(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du médecin est invalide.", nameof(id));
            }

            var medecin = await _medecinRepository.GetByIdAsync(id);
            if (medecin == null)
            {
                throw new KeyNotFoundException("Médecin introuvable.");
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
            if (medecins == null || !medecins.Any())
            {
                throw new InvalidOperationException("Aucun médecin trouvé.");
            }

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
            if (id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du médecin est invalide.", nameof(id));
            }

            if (medecinDto == null)
            {
                throw new ArgumentNullException(nameof(medecinDto), "Les données du médecin ne peuvent pas être nulles.");
            }

            var medecin = await _medecinRepository.GetByIdAsync(id);
            if (medecin == null)
            {
                throw new KeyNotFoundException("Médecin introuvable.");
            }

            medecin.Prenom = medecinDto.Prenom;
            medecin.Nom = medecinDto.Nom;
            medecin.Specialite = medecinDto.Specialite;
            medecin.CliniqueId = medecinDto.CliniqueId ?? Guid.Empty;
            medecin.Email = medecinDto.Email;
            medecin.Telephone = medecinDto.Telephone;
            medecin.PhotoUrl = medecinDto.PhotoUrl;
            medecin.Disponibilites = medecinDto.Disponibilites;

            await _medecinRepository.UpdateAsync(medecin);
        }

        public async Task DeleteDoctor(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du médecin est invalide.", nameof(id));
            }

            var medecin = await _medecinRepository.GetByIdAsync(id);
            if (medecin == null)
            {
                throw new KeyNotFoundException("Médecin introuvable.");
            }

            await _medecinRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<MedecinDto>> FilterDoctorsBySpecialite(string specialite)
        {
            if (string.IsNullOrWhiteSpace(specialite))
            {
                throw new ArgumentException("La spécialité ne peut pas être vide.", nameof(specialite));
            }

            var medecins = await _medecinRepository.FilterBySpecialiteAsync(specialite);
            if (medecins == null || !medecins.Any())
            {
                throw new InvalidOperationException("Aucun médecin trouvé pour la spécialité spécifiée.");
            }

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

        public async Task<IEnumerable<MedecinDto>> FilterDoctorsByName(string name, string prenom)
        {
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(prenom))
            {
                throw new ArgumentException("Le nom ou le prénom doit être spécifié.");
            }

            var medecins = await _medecinRepository.FilterByNameOrPrenomAsync(name, prenom);
            if (medecins == null || !medecins.Any())
            {
                throw new InvalidOperationException("Aucun médecin trouvé pour les critères spécifiés.");
            }

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

        public async Task<IEnumerable<MedecinDto>> GetMedecinByClinique(Guid cliniqueId)
        {
            if (cliniqueId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant de la clinique est invalide.", nameof(cliniqueId));
            }

            var medecins = await _medecinRepository.GetMedecinByCliniqueIdAsync(cliniqueId);
            if (medecins == null || !medecins.Any())
            {
                throw new InvalidOperationException("Aucun médecin trouvé pour la clinique spécifiée.");
            }

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

        public async Task AttribuerMedecinAUneClinique(Guid medecinId, Guid cliniqueId)
        {
            if (medecinId == Guid.Empty || cliniqueId == Guid.Empty)
            {
                throw new ArgumentException("Les identifiants du médecin et de la clinique doivent être valides.");
            }

            var medecin = await _medecinRepository.GetByIdAsync(medecinId);
            if (medecin == null)
            {
                throw new KeyNotFoundException("Médecin introuvable.");
            }

            await _medecinRepository.AttribuerMedecinAUneCliniqueAsync(medecinId, cliniqueId);
        }

        public async Task DesabonnerMedecinDeClinique(Guid medecinId)
        {
            if (medecinId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du médecin est invalide.", nameof(medecinId));
            }

            var medecin = await _medecinRepository.GetByIdAsync(medecinId);
            if (medecin == null)
            {
                throw new KeyNotFoundException("Médecin introuvable.");
            }

            await _medecinRepository.DesabonnerMedecinDeCliniqueAsync(medecinId);
        }

        public async Task<IEnumerable<StatistiqueMedecinDTO>> GetNombreMedecinBySpecialite()
        {
            var statistiques = await _medecinRepository.GetNombreMedecinBySpecialiteAsync();
            if (statistiques == null || !statistiques.Any())
            {
                throw new InvalidOperationException("Aucune statistique trouvée.");
            }
            return statistiques.Select(s => new StatistiqueMedecinDTO
            {
                Cle = s.Cle,
                Nombre = s.Nombre
            });
        }

        public async Task<IEnumerable<StatistiqueMedecinDTO>> GetNombreMedecinByClinique()
        {
            var statistiques = await _medecinRepository.GetNombreMedecinByCliniqueAsync();
            if (statistiques == null || !statistiques.Any())
            {
                throw new InvalidOperationException("Aucune statistique trouvée.");
            }
            return statistiques.Select(s => new StatistiqueMedecinDTO
            {
                Cle = s.Cle,
                Nombre = s.Nombre
            });
        }

        public async Task<IEnumerable<StatistiqueMedecinDTO>> GetNombreMedecinBySpecialiteDansUneClinique(Guid cliniqueId)
        {
            if (cliniqueId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant de la clinique est invalide.", nameof(cliniqueId));
            }
            var statistiques = await _medecinRepository.GetNombreMedecinBySpecialiteDansUneCliniqueAsync(cliniqueId);
            if (statistiques == null || !statistiques.Any())
            {
                throw new InvalidOperationException("Aucune statistique trouvée.");
            }
            return statistiques.Select(s => new StatistiqueMedecinDTO
            {
                Cle = s.Cle,
                Nombre = s.Nombre
            });
        }

        public async Task<IEnumerable<Guid>> GetMedecinsIdsByCliniqueId(Guid cliniqueId)
        {
            if (cliniqueId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant de la clinique est invalide.", nameof(cliniqueId));
            }
            var medecinsIds = await _medecinRepository.GetMedecinsIdsByCliniqueId(cliniqueId);
            if (medecinsIds == null || !medecinsIds.Any())
            {
                throw new InvalidOperationException("Aucun médecin trouvé pour la clinique spécifiée.");
            }
            return medecinsIds;
        }


        public async Task<IEnumerable<ActiviteMedecinDTO>> GetActivitesMedecin(Guid medecinId)
        {
            if (medecinId == Guid.Empty)
            {
                throw new ArgumentException("L'identifiant du médecin est invalide.", nameof(medecinId));
            }
            var activites = await _medecinRepository.GetActivitesMedecinAsync(medecinId);
            if (activites == null || !activites.Any())
            {
                throw new InvalidOperationException("Aucune activité trouvée pour le médecin spécifié.");
            }
            return activites.Select(a => new ActiviteMedecinDTO
            {
                MedecinId = a.MedecinId,
                NomComplet = a.NomComplet,
                NombreConsultations = a.NombreConsultations,
                NombreRendezVous = a.NombreRendezVous,
            });
        }
    }
}
