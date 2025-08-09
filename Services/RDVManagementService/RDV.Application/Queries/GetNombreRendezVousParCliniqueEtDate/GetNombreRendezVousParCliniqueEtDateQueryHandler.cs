using MediatR;
using Microsoft.Extensions.Logging;
using RDV.Domain.Interfaces;
using System.Text.Json;

namespace RDV.Application.Queries.GetNombreRendezVousParCliniqueEtDate
{
    public class GetNombreRendezVousParCliniqueEtDateQueryHandler : IRequestHandler<GetNombreRendezVousParCliniqueEtDateQuery, int>
    {
        private readonly IRendezVousRepository _rendezVousRepository;
        private readonly HttpClient _httpClient;
        private readonly ILogger<GetNombreRendezVousParCliniqueEtDateQueryHandler> _logger;

        public GetNombreRendezVousParCliniqueEtDateQueryHandler(
            IRendezVousRepository rendezVousRepository,
            IHttpClientFactory httpClientFactory,
            ILogger<GetNombreRendezVousParCliniqueEtDateQueryHandler> logger)
        {
            _rendezVousRepository = rendezVousRepository;
            _httpClient = httpClientFactory.CreateClient("DoctorService");
            _logger = logger;
        }

        public async Task<int> Handle(GetNombreRendezVousParCliniqueEtDateQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Appel à DoctorService pour récupérer les IDs des médecins de la clinique
                var response = await _httpClient.GetAsync($"medecinsIds/clinique/{request.cliniqueId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var medecinIds = JsonSerializer.Deserialize<List<Guid>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (medecinIds == null || !medecinIds.Any())
                    return 0;

                return await _rendezVousRepository.CountByMedecinsIdsAndDateAsync(medecinIds, request.date);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du nombre de rendez-vous");
                throw;
            }
        }
    }

}
