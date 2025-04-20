using System.Text;
using System.Text.Json;
using NextUp.Models;
using Microsoft.AspNetCore.Http;

namespace NextUp.Services
{
    public class IgdbService
    {
        private readonly HttpClient _httpClient;
        private readonly IgdbAuthService _authService;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Modify constructor to accept IHttpContextAccessor
        public IgdbService(HttpClient httpClient, IgdbAuthService authService, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _authService = authService;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Game>> SearchGamesAsync(string gameTitle)
        {
            // fetch game being searched for
            if (string.IsNullOrWhiteSpace(gameTitle))
            {
                return new List<Game>();
            }
            var escapedTitle = gameTitle.Replace("\"", "");
            var igdbGames = await FetchGamesAsync($"search \"{escapedTitle}\"; where version_parent = null;");
            return MapToGames(igdbGames);
        }

        public async Task<List<Game>> FetchUpcomingGamesAsync()
        {
            // fetch hyped games coming soon
            var igdbGames = await FetchGamesAsync("hypes; where first_release_date > " + ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() +
                "& hypes != null & hypes > 20; sort first_release_date asc;");
            return MapToGames(igdbGames);
        }

        public async Task<List<Game>> FetchNewReleasesAsync()
        {
            // fetch newly released games with a total rating > 70
            var igdbGames = await FetchGamesAsync("total_rating; where first_release_date <= " + ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() +
                "& total_rating > 70; sort first_release_date desc;");
            return MapToGames(igdbGames);
        }

        private async Task<List<IgdbGameDto>> FetchGamesAsync(string whereClause)
        {
            var token = await _authService.GetAccessTokenAsync();
            var clientId = _config["Igdb:ClientId"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", clientId);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var query = $"fields name,first_release_date,cover.url,platforms.name; {whereClause} limit 10;";
            var content = new StringContent(query, Encoding.UTF8, "text/plain");

            var response = await _httpClient.PostAsync("https://api.igdb.com/v4/games", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<IgdbGameDto>>(json)!;
        }

        private List<Game> MapToGames(List<IgdbGameDto> igdbGames)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value; // Get the UserId from the authenticated user's claims
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name; // Get the UserName (or any other information)

            return igdbGames.Select(g => new Game
            {
                Title = g.name,
                CoverImageUrl = g.cover?.url != null ? $"https:{g.cover.url.Replace("t_thumb", "t_cover_big")}" : "",
                Platform = g.platforms != null ? string.Join(", ", g.platforms.Select(p => p.name)) : "Unknown",
                ReleaseDate = g.first_release_date.HasValue ? DateTimeOffset.FromUnixTimeSeconds(g.first_release_date.Value).DateTime : null,
                UserId = userId, // Set the UserId from the current user
                User = new ApplicationUser { Id = userId, UserName = userName } // Set the User object with Id and UserName
            }).ToList();
        }
    }
}
