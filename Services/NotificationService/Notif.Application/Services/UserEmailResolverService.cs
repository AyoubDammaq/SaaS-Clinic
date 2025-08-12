using Microsoft.Extensions.Logging;
using Notif.Application.Interfaces;
using Notif.Domain.Enums;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Notif.Application.Services
{
    public class UserEmailResolverService : IUserEmailResolverService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserEmailResolverService> _logger;

        public UserEmailResolverService(HttpClient httpClient, ILogger<UserEmailResolverService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        private class UserResponse
        {
            public string? Email { get; set; }
        }


        public async Task<string?> GetUserEmailAsync(Guid id, UserType userType)
        {
            try
            {
                string endpoint = userType switch
                {
                    UserType.Patient => $"http://patientservice:8087/api/Patients/{id}",
                    UserType.Doctor => $"http://doctorservice:8085/api/Medecin/{id}",
                    _ => throw new ArgumentOutOfRangeException(nameof(userType), $"Unsupported user type {userType}")
                };

                _logger.LogInformation("Fetching email for user {id} from {Endpoint}", id, endpoint);

                var response = await _httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Unable to fetch user {UserId} of type {UserType}", id, userType);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received JSON: {Json}", json);

                var user = JsonSerializer.Deserialize<UserResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (!string.IsNullOrWhiteSpace(user?.Email))
                {
                    _logger.LogInformation("Extracted email: {Email}", user.Email);
                    return user.Email;
                }

                _logger.LogWarning("Email not found in deserialized response");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user email for {UserId}", id);
                return null;
            }
        }
    }
}
