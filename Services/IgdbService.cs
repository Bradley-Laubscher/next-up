using System.Text;
using System.Text.Json;

namespace NextUp.Services
{
    public class IgdbService
    {
        private readonly HttpClient _httpClient;
        private readonly IgdbAuthService _authService;
        private readonly IConfiguration _config;

        public IgdbService(HttpClient httpClient, IgdbAuthService authService, IConfiguration config)
        {
            _httpClient = httpClient;
            _authService = authService;
            _config = config;
        }

        public async Task<string> FetchUpcomingGamesAsync()
        {
            var token = await _authService.GetAccessTokenAsync();
            var clientId = _config["Igdb:ClientId"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", clientId);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var query = "fields name,first_release_date,cover.url,platforms.name; where first_release_date > " + ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() + "; sort first_release_date asc; limit 10;";
            var content = new StringContent(query, Encoding.UTF8, "text/plain");

            var response = await _httpClient.PostAsync("https://api.igdb.com/v4/games", content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
