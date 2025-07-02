using Reporting.Domain.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Reporting.Infrastructure.Repositories
{
    public class ConsultationStateRepository : IConsultationStateRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ConsultationStateRepository> _logger;

        public ConsultationStateRepository(HttpClient httpClient, ILogger<ConsultationStateRepository> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<ConsultationDataModel>> GetAllConsultationsAsync(Guid? cliniqueId)
        {
            var query = cliniqueId.HasValue ? $"?cliniqueId={cliniqueId}" : "";
            var response = await _httpClient.GetAsync($"/api/Consultation");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Erreur lors de la récupération des consultations : {status}", response.StatusCode);
                return new List<ConsultationDataModel>();
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ConsultationDataModel>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<ConsultationDataModel>();
        }
    }
}
