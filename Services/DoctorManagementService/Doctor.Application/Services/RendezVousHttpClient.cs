using Doctor.Application.DTOs;
using Doctor.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;


namespace Doctor.Application.Services
{
    public class RendezVousHttpClient : IRendezVousHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RendezVousHttpClient> _logger;

        public RendezVousHttpClient(HttpClient httpClient, ILogger<RendezVousHttpClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<RendezVousDTO>> GetRendezVousParMedecinEtDate(Guid medecinId, DateTime date)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/rendezvous/medecin/{medecinId}/date?date={date:O}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<RendezVousDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'appel au RDVService");
                return new();
            }
        }
    }
}
