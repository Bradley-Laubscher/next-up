using System.Text.Json;

namespace NextUp.Services
{
    public class IgdbAuthService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        private string? _accessToken;
        private DateTime _expiresAt;

        public IgdbAuthService(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (_accessToken != null && _expiresAt > DateTime.UtcNow)
            {
                return _accessToken;
            }

            var clientId = _config["Igdb:ClientId"];
            var clientSecret = _config["Igdb:ClientSecret"];

            var response = await _httpClient.PostAsync($"https://id.twitch.tv/oauth2/token?client_id={clientId}&client_secret={clientSecret}&grant_type=client_credentials", null);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TwitchTokenResponse>(content);

            _accessToken = result?.access_token;
            _expiresAt = DateTime.UtcNow.AddSeconds(result?.expires_in ?? 3600);

            return _accessToken!;
        }

        private class TwitchTokenResponse
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
        }
    }
}
