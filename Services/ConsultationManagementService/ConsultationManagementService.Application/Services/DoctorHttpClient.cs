using ConsultationManagementService.Application.DTOs;
using ConsultationManagementService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ConsultationManagementService.Application.Services
{
    public class DoctorHttpClient : IDoctorHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DoctorHttpClient> _logger;

        public DoctorHttpClient(HttpClient httpClient, ILogger<DoctorHttpClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<GetMedecinDto> GetDoctorById(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/medecin/{id}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GetMedecinDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'appel au DoctorService");
                return new();
            }
        }
    }
}
