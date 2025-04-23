using Reporting.Application.DTOs;
using Reporting.Application.Interfaces;
using Reporting.Domain.Interfaces;

namespace Reporting.Application.Services
{
    public class ReportingService : IReportingService
    {
        private readonly IReportingRepository _repository;

        public ReportingService(IReportingRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> GetNombreConsultationsAsync(DateTime dateDebut, DateTime dateFin)
        {
            return await _repository.GetNombreConsultationsAsync(dateDebut, dateFin);
        }

        public async Task<IEnumerable<RendezVousStatDTO>> GetStatistiquesRendezVousAsync(DateTime dateDebut, DateTime dateFin)
        {
            if (dateDebut > dateFin)
            {
                throw new InvalidOperationException("La date de début ne peut pas être supérieure à la date de fin.");
            }

            var stats = await _repository.GetStatistiquesRendezVousAsync(dateDebut, dateFin);

            if (stats == null || !stats.Any())
            {
                throw new ArgumentException("Aucune statistique de rendez-vous trouvée pour la période spécifiée.");
            }

            return stats.Select(stat => new RendezVousStatDTO
            {
                Date = stat.Date,
                TotalRendezVous = stat.TotalRendezVous,
                Confirmes = stat.Confirmes,
                Annules = stat.Annules,
                EnAttente = stat.EnAttente
            });
        }

        public async Task<int> GetNombreNouveauxPatientsAsync(DateTime dateDebut, DateTime dateFin)
        {
            return await _repository.GetNombreNouveauxPatientsAsync(dateDebut, dateFin);
        }

        public async Task<List<DoctorStatsDTO>> GetNombreMedecinParSpecialite()
        {
            var statsList = await _repository.GetNombreMedecinParSpecialiteAsync();

            return statsList.Select(stat => new DoctorStatsDTO
            {
                Cle = stat.Cle,
                Nombre = stat.Nombre
            }).ToList();
        }


        public async Task<List<DoctorStatsDTO>> GetNombreMedecinByClinique()
        {
            var stats = await _repository.GetNombreMedecinByCliniqueAsync();
            return stats.Select(stat => new DoctorStatsDTO
            {
                Cle = stat.Cle,
                Nombre = stat.Nombre
            }).ToList();
        }

        public async Task<List<DoctorStatsDTO>> GetNombreMedecinBySpecialiteDansUneClinique(Guid cliniqueId)
        {
            var stats = await _repository.GetNombreMedecinBySpecialiteDansUneCliniqueAsync(cliniqueId);
            return stats.Select(stat => new DoctorStatsDTO
            {
                Cle = stat.Cle,
                Nombre = stat.Nombre
            }).ToList();
        }

        public async Task<List<FactureStatsDTO>> GetNombreDeFactureByStatus()
        {
            var stats = await _repository.GetNombreDeFactureByStatusAsync();
            return stats.Select(stat => new FactureStatsDTO
            {
                Cle = stat.Cle,
                Nombre = stat.Nombre
            }).ToList();
        }

        public async Task<List<FactureStatsDTO>> GetNombreDeFactureParClinique()
        {
            var stats = await _repository.GetNombreDeFactureParCliniqueAsync();
            return stats.Select(stat => new FactureStatsDTO
            {
                Cle = stat.Cle,
                Nombre = stat.Nombre
            }).ToList();
        }

        public async Task<List<FactureStatsDTO>> GetNombreDeFactureParStatusParClinique()
        {
            var stats = await _repository.GetNombreDeFacturesByStatusParCliniqueAsync();
            return stats.Select(stat => new FactureStatsDTO
            {
                Cle = stat.Cle,
                Nombre = stat.Nombre
            }).ToList();
        }

        public async Task<List<FactureStatsDTO>> GetNombreDeFacturesByStatusDansUneClinique(Guid cliniqueId)
        {
            var stats = await _repository.GetNombreDeFacturesByStatusDansUneCliniqueAsync(cliniqueId);
            return stats.Select(stat => new FactureStatsDTO
            {
                Cle = stat.Cle,
                Nombre = stat.Nombre
            }).ToList();
        }


        public async Task<int> GetNombreDeCliniques()
        {
            return await _repository.GetNombreDeCliniquesAsync();
        }

        public async Task<int> GetNombreNouvellesCliniquesDuMois()
        {
            return await _repository.GetNombreNouvellesCliniquesDuMoisAsync();
        }

        public async Task<IEnumerable<StatistiqueDTO>> GetNombreNouvellesCliniquesParMois()
        {
            var stats = await _repository.GetNombreNouvellesCliniquesParMoisAsync();
            return stats.Select(stat => new StatistiqueDTO
            {
                Cle = stat.Cle,
                Nombre = stat.Nombre
            });
        }

        public async Task<StatistiqueCliniqueDTO> GetStatistiquesClinique(Guid cliniqueId)
        {
            var stats = await _repository.GetStatistiquesCliniqueAsync(cliniqueId);
            return new StatistiqueCliniqueDTO
            {
                CliniqueId = stats.CliniqueId,
                Nom = stats.Nom,
                NombreMedecins = stats.NombreMedecins,
                NombrePatients = stats.NombrePatients,
                NombreRendezVous = stats.NombreRendezVous,
                NombreConsultations = stats.NombreConsultations,
            };
        }

        public async Task<IEnumerable<ActiviteMedecinDTO>> GetActivitesMedecin(Guid medecinId)
        {
            var activites = await _repository.GetActivitesMedecinAsync(medecinId);
            return activites.Select(activite => new ActiviteMedecinDTO
            {
                MedecinId = activite.MedecinId,
                NomComplet = activite.NomComplet,
                NombreConsultations = activite.NombreConsultations,
                NombreRendezVous = activite.NombreRendezVous
            });
        }









        public async Task<decimal> GetMontantPaiementsAsync(string statut, DateTime dateDebut, DateTime dateFin)
        {
            return await _repository.GetMontantPaiementsAsync(statut, dateDebut, dateFin);
        }

        public async Task<int> GetNombreFacturesAsync(DateTime dateDebut, DateTime dateFin)
        {
            return await _repository.GetNombreFacturesAsync(dateDebut, dateFin);
        }

        public async Task<decimal> GetMontantFacturesAsync(string statut, DateTime dateDebut, DateTime dateFin)
        {
            return await _repository.GetMontantFacturesAsync(statut, dateDebut, dateFin);
        }

        public async Task<DashboardStatsDTO> GetDashboardStatsAsync(DateTime dateDebut, DateTime dateFin)
        {
            var data = await _repository.GetDashboardStatsAsync(dateDebut, dateFin);

            return new DashboardStatsDTO
            {
                ConsultationsJour = data.ConsultationsJour,
                NouveauxPatients = data.NouveauxPatients,
                NombreFactures = data.NombreFactures,
                TotalFacturesPayees = data.TotalFacturesPayees,
                TotalFacturesImpayees = data.TotalFacturesImpayees,
                PaiementsPayes = data.PaiementsPayes,
                PaiementsImpayes = data.PaiementsImpayes,
                PaiementsEnAttente = data.PaiementsEnAttente
            };
        }
    }


}
