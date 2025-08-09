using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Reporting.Application.DTOs;
using Reporting.Application.Interfaces;
using Reporting.Domain.Interfaces;

namespace Reporting.Application.Services
{
    public class ReportingService : IReportingService
    {
        private readonly IReportingRepository _repository;
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;
        private readonly ILogger<ReportingService> _logger;
        private static readonly TimeSpan _defaultCacheDuration = TimeSpan.FromMinutes(10);

        public ReportingService(IReportingRepository repository, IMemoryCache cache, IMapper mapper, ILogger<ReportingService> logger)
        {
            _repository = repository;
            _cache = cache;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> GetNombreConsultationsAsync(DateTime dateDebut, DateTime dateFin)
        {
            var cacheKey = $"Consultations:{dateDebut:yyyyMMdd}:{dateFin:yyyyMMdd}";
            if (_cache.TryGetValue(cacheKey, out int cachedResult))
            {
                _logger.LogInformation("Cache hit pour GetNombreConsultationsAsync avec la clé {CacheKey}", cacheKey);
                return cachedResult;
            }

            _logger.LogInformation("Cache miss pour GetNombreConsultationsAsync avec la clé {CacheKey}", cacheKey);
            var result = await _repository.GetNombreConsultationsAsync(dateDebut, dateFin);
            _cache.Set(cacheKey, result, _defaultCacheDuration);
            return result;
        }

        public async Task<IEnumerable<RendezVousStatDTO>> GetStatistiquesRendezVousAsync(DateTime dateDebut, DateTime dateFin)
        {
            if (dateDebut > dateFin)
            {
                _logger.LogWarning("Date de début supérieure à la date de fin dans GetStatistiquesRendezVousAsync : {DateDebut} > {DateFin}", dateDebut, dateFin);
                throw new InvalidOperationException("La date de début ne peut pas être supérieure à la date de fin.");
            }

            var cacheKey = $"RdvStats:{dateDebut:yyyyMMdd}:{dateFin:yyyyMMdd}";
            if (_cache.TryGetValue(cacheKey, out IEnumerable<RendezVousStatDTO> cachedStats))
            {
                _logger.LogInformation("Cache hit pour GetStatistiquesRendezVousAsync avec la clé {CacheKey}", cacheKey);
                return cachedStats;
            }

            _logger.LogInformation("Cache miss pour GetStatistiquesRendezVousAsync avec la clé {CacheKey}", cacheKey);
            var stats = await _repository.GetStatistiquesRendezVousAsync(dateDebut, dateFin);
            if (stats == null || !stats.Any())
            {
                _logger.LogWarning("Aucune statistique de rendez-vous trouvée pour la période {DateDebut} - {DateFin}", dateDebut, dateFin);
                throw new ArgumentException("Aucune statistique de rendez-vous trouvée pour la période spécifiée.");
            }

            var result = _mapper.Map<List<RendezVousStatDTO>>(stats);
            _cache.Set(cacheKey, result, _defaultCacheDuration);
            return result;
        }

        public async Task<int> GetNombreNouveauxPatientsAsync(DateTime dateDebut, DateTime dateFin)
        {
            _logger.LogInformation("Appel de GetNombreNouveauxPatientsAsync pour la période {DateDebut} - {DateFin}", dateDebut, dateFin);
            return await _repository.GetNombreNouveauxPatientsAsync(dateDebut, dateFin);
        }

        public async Task<List<DoctorStatsDTO>> GetNombreMedecinParSpecialite()
        {
            _logger.LogInformation("Appel de GetNombreMedecinParSpecialite");
            var stats = await _repository.GetNombreMedecinParSpecialiteAsync();
            return _mapper.Map<List<DoctorStatsDTO>>(stats);
        }

        public async Task<List<DoctorStatsDTO>> GetNombreMedecinByClinique()
        {
            _logger.LogInformation("Appel de GetNombreMedecinByClinique");
            var stats = await _repository.GetNombreMedecinByCliniqueAsync();
            return _mapper.Map<List<DoctorStatsDTO>>(stats);
        }

        public async Task<List<DoctorStatsDTO>> GetNombreMedecinBySpecialiteDansUneClinique(Guid cliniqueId)
        {
            _logger.LogInformation("Appel de GetNombreMedecinBySpecialiteDansUneClinique pour la clinique {CliniqueId}", cliniqueId);
            var stats = await _repository.GetNombreMedecinBySpecialiteDansUneCliniqueAsync(cliniqueId);
            return _mapper.Map<List<DoctorStatsDTO>>(stats);
        }

        public async Task<List<FactureStatsDTO>> GetNombreDeFactureByStatus()
        {
            _logger.LogInformation("Appel de GetNombreDeFactureByStatus");
            var stats = await _repository.GetNombreDeFactureByStatusAsync();
            return _mapper.Map<List<FactureStatsDTO>>(stats);
        }

        public async Task<List<FactureStatsDTO>> GetNombreDeFactureParClinique()
        {
            _logger.LogInformation("Appel de GetNombreDeFactureParClinique");
            var stats = await _repository.GetNombreDeFactureParCliniqueAsync();
            return _mapper.Map<List<FactureStatsDTO>>(stats);
        }

        public async Task<List<FactureStatsDTO>> GetNombreDeFactureParStatusParClinique()
        {
            _logger.LogInformation("Appel de GetNombreDeFactureParStatusParClinique");
            var stats = await _repository.GetNombreDeFacturesByStatusParCliniqueAsync();
            return _mapper.Map<List<FactureStatsDTO>>(stats);
        }

        public async Task<List<FactureStatsDTO>> GetNombreDeFacturesByStatusDansUneClinique(Guid cliniqueId)
        {
            _logger.LogInformation("Appel de GetNombreDeFacturesByStatusDansUneClinique pour la clinique {CliniqueId}", cliniqueId);
            var stats = await _repository.GetNombreDeFacturesByStatusDansUneCliniqueAsync(cliniqueId);
            return _mapper.Map<List<FactureStatsDTO>>(stats);
        }

        public async Task<int> GetNombreDeCliniques()
        {
            _logger.LogInformation("Appel de GetNombreDeCliniques");
            return await _repository.GetNombreDeCliniquesAsync();
        }

        public async Task<int> GetNombreNouvellesCliniquesDuMois()
        {
            _logger.LogInformation("Appel de GetNombreNouvellesCliniquesDuMois");
            return await _repository.GetNombreNouvellesCliniquesDuMoisAsync();
        }

        public async Task<IEnumerable<StatistiqueDTO>> GetNombreNouvellesCliniquesParMois()
        {
            _logger.LogInformation("Appel de GetNombreNouvellesCliniquesParMois");
            var stats = await _repository.GetNombreNouvellesCliniquesParMoisAsync();
            return _mapper.Map<List<StatistiqueDTO>>(stats);
        }

        public async Task<StatistiqueCliniqueDTO> GetStatistiquesClinique(Guid cliniqueId)
        {
            _logger.LogInformation("Appel de GetStatistiquesClinique pour la clinique {CliniqueId}", cliniqueId);
            var stats = await _repository.GetStatistiquesCliniqueAsync(cliniqueId);
            return _mapper.Map<StatistiqueCliniqueDTO>(stats);
        }

        public async Task<IEnumerable<ActiviteMedecinDTO>> GetActivitesMedecin(Guid medecinId)
        {
            _logger.LogInformation("Appel de GetActivitesMedecin pour le médecin {MedecinId}", medecinId);
            var activites = await _repository.GetActivitesMedecinAsync(medecinId);
            return _mapper.Map<List<ActiviteMedecinDTO>>(activites);
        }

        public async Task<decimal> GetMontantPaiementsAsync(string statut, DateTime dateDebut, DateTime dateFin)
        {
            _logger.LogInformation("Appel de GetMontantPaiementsAsync pour le statut {Statut} et la période {DateDebut} - {DateFin}", statut, dateDebut, dateFin);
            return await _repository.GetMontantPaiementsAsync(statut, dateDebut, dateFin);
        }

        public async Task<StatistiquesFactureDto> GetStatistiquesFacturesAsync(DateTime dateDebut, DateTime dateFin)
        {
            _logger.LogInformation("Appel de GetStatistiquesFacturesAsync pour la période {DateDebut} - {DateFin}", dateDebut, dateFin);
            var stats = await _repository.GetStatistiquesFacturesAsync(dateDebut, dateFin);
            return _mapper.Map<StatistiquesFactureDto>(stats);
        }   

        public async Task<List<ComparaisonCliniqueDTO>> ComparerCliniquesAsync(List<Guid> cliniqueIds)
        {
            if (cliniqueIds == null || !cliniqueIds.Any())
            {
                _logger.LogWarning("ComparerCliniquesAsync appelé avec une liste vide ou nulle.");
                return new List<ComparaisonCliniqueDTO>();
            }

            var cacheKey = $"ComparaisonCliniques:{string.Join("-", cliniqueIds.OrderBy(id => id))}";

            if (_cache.TryGetValue(cacheKey, out List<ComparaisonCliniqueDTO> cachedResult))
            {
                _logger.LogInformation("Cache hit pour ComparerCliniquesAsync avec la clé {CacheKey}", cacheKey);
                return cachedResult;
            }

            _logger.LogInformation("Cache miss pour ComparerCliniquesAsync avec la clé {CacheKey}", cacheKey);
            var result = new List<ComparaisonCliniqueDTO>();

            foreach (var id in cliniqueIds)
            {
                var stats = await _repository.GetStatistiquesCliniqueAsync(id);
                var dto = _mapper.Map<ComparaisonCliniqueDTO>(stats);
                result.Add(dto);
            }

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }

        public async Task<IEnumerable<AppointmentDayStatDto>> GetStatistiquesHebdomadairesRendezVousByDoctorAsync(Guid medecinId, DateTime dateDebut, DateTime dateFin)
        {
            if (dateDebut > dateFin)
            {
                _logger.LogWarning("Date de début supérieure à la date de fin dans GetStatistiquesHebdomadairesRendezVousByDoctorAsync : {DateDebut} > {DateFin}", dateDebut, dateFin);
                throw new InvalidOperationException("La date de début ne peut pas être supérieure à la date de fin.");
            }

            var cacheKey = $"RdvHebdoStats:{dateDebut:yyyyMMdd}:{dateFin:yyyyMMdd}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<AppointmentDayStatDto> cachedStats))
            {
                _logger.LogInformation("Cache hit pour GetStatistiquesHebdomadairesRendezVousByDoctorAsync avec la clé {CacheKey}", cacheKey);
                return cachedStats;
            }

            _logger.LogInformation("Cache miss pour GetStatistiquesHebdomadairesRendezVousByDoctorAsync avec la clé {CacheKey}", cacheKey);

            var stats = await _repository.GetStatistiquesHebdomadairesRendezVousByDoctorAsync(medecinId, dateDebut, dateFin);

            var result = _mapper.Map<List<AppointmentDayStatDto>>(stats);
            _cache.Set(cacheKey, result, _defaultCacheDuration);

            return result;
        }

        public async Task<IEnumerable<AppointmentDayStatDto>> GetStatistiquesHebdomadairesRendezVousByClinicAsync(Guid cliniqueId, DateTime dateDebut, DateTime dateFin)
        {
            if (dateDebut > dateFin)
            {
                _logger.LogWarning("Date de début supérieure à la date de fin dans GetStatistiquesHebdomadairesRendezVousByClinicAsync : {DateDebut} > {DateFin}", dateDebut, dateFin);
                throw new InvalidOperationException("La date de début ne peut pas être supérieure à la date de fin.");
            }

            var cacheKey = $"RdvHebdoStats:{dateDebut:yyyyMMdd}:{dateFin:yyyyMMdd}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<AppointmentDayStatDto> cachedStats))
            {
                _logger.LogInformation("Cache hit pour GetStatistiquesHebdomadairesRendezVousByClinicAsync avec la clé {CacheKey}", cacheKey);
                return cachedStats;
            }

            _logger.LogInformation("Cache miss pour GetStatistiquesHebdomadairesRendezVousByClinicAsync avec la clé {CacheKey}", cacheKey);

            var stats = await _repository.GetStatistiquesHebdomadairesRendezVousByClinicAsync(cliniqueId, dateDebut, dateFin);

            var result = _mapper.Map<List<AppointmentDayStatDto>>(stats);
            _cache.Set(cacheKey, result, _defaultCacheDuration);

            return result;
        }

        public async Task<DashboardStatsDTO> GetDashboardStatsAsync(DateTime dateDebut, DateTime dateFin, Guid? patientId = null, Guid? medecinId = null, Guid? cliniqueId = null)
        {
            var cacheKey = $"DashboardStats:{dateDebut:yyyyMMdd}:{dateFin:yyyyMMdd}:{patientId}:{medecinId}:{cliniqueId}";

            try
            {
                if (_cache.TryGetValue(cacheKey, out DashboardStatsDTO cachedStats))
                {
                    _logger.LogInformation("Cache hit pour GetDashboardStatsAsync avec la clé {CacheKey}", cacheKey);
                    return cachedStats;
                }

                _logger.LogInformation("Cache miss pour GetDashboardStatsAsync avec la clé {CacheKey}", cacheKey);
                var data = await _repository.GetDashboardStatsAsync(dateDebut, dateFin, patientId, medecinId, cliniqueId);
                var result = _mapper.Map<DashboardStatsDTO>(data);
                _cache.Set(cacheKey, result, _defaultCacheDuration);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'exécution de GetDashboardStatsAsync pour la période {DateDebut} - {DateFin}", dateDebut, dateFin);
                throw;
            }
        }
    }
}
