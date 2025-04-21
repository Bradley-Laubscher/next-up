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

        // fetch game being searched for
        public async Task<List<Game>> SearchGamesAsync(string gameTitle)
        {
            if (string.IsNullOrWhiteSpace(gameTitle))
            {
                return new List<Game>();
            }
            var escapedTitle = gameTitle.Replace("\"", "");
            var igdbGames = await FetchGamesAsync($"search \"{escapedTitle}\"; where version_parent = null;");
            return MapToGames(igdbGames);
        }

        // fetch hyped games coming soon
        public async Task<List<Game>> FetchUpcomingGamesAsync()
        {
            var igdbGames = await FetchGamesAsync("hypes; where first_release_date > " + ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() +
                "& hypes != null & hypes > 20; sort first_release_date asc;");
            return MapToGames(igdbGames);
        }

        // fetch newly released games with a total rating > 70
        public async Task<List<Game>> FetchNewReleasesAsync()
        {
            var igdbGames = await FetchGamesAsync("total_rating; where first_release_date <= " + ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() +
                "& total_rating > 70; sort first_release_date desc;");
            return MapToGames(igdbGames);
        }

        private async Task<List<IgdbGameDto>> FetchGamesAsync(string whereClause)
        {
            var token = await _authService.GetAccessTokenAsync();
            var clientId = _config["IGDB_ClientId"];

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

        public async Task<string?> GetUpcomingUpdateInfoAsync(string gameTitle)
        {
            if (string.IsNullOrWhiteSpace(gameTitle))
                return null;

            var token = await _authService.GetAccessTokenAsync();
            var clientId = _config["IGDB_ClientId"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Client-ID", clientId);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // Step 1: Get the main game and its ID
            var baseGameQuery = $"fields id; search \"{gameTitle}\"; where version_parent = null; limit 1;";
            var baseGameContent = new StringContent(baseGameQuery, Encoding.UTF8, "text/plain");

            var baseGameResponse = await _httpClient.PostAsync("https://api.igdb.com/v4/games", baseGameContent);
            baseGameResponse.EnsureSuccessStatusCode();
            var baseGameJson = await baseGameResponse.Content.ReadAsStringAsync();

            var baseGame = JsonSerializer.Deserialize<List<IgdbGameDto>>(baseGameJson)?.FirstOrDefault();
            if (baseGame == null) return null;

            // Step 2: Get expansions/DLCs with that ID as version_parent
            var currentUnixTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            var expansionQuery = $@"
                fields name, first_release_date; 
                where version_parent = {baseGame.id} & first_release_date > {currentUnixTime}; 
                sort first_release_date asc; 
                limit 1;
            ";
            var expansionContent = new StringContent(expansionQuery, Encoding.UTF8, "text/plain");

            var expansionResponse = await _httpClient.PostAsync("https://api.igdb.com/v4/games", expansionContent);
            expansionResponse.EnsureSuccessStatusCode();
            var expansionJson = await expansionResponse.Content.ReadAsStringAsync();

            var expansions = JsonSerializer.Deserialize<List<IgdbGameDto>>(expansionJson);
            var upcomingExpansion = expansions?.FirstOrDefault();

            if (upcomingExpansion != null && upcomingExpansion.first_release_date.HasValue)
            {
                var releaseDate = DateTimeOffset.FromUnixTimeSeconds(upcomingExpansion.first_release_date.Value).DateTime;
                return $"{upcomingExpansion.name} - {releaseDate.ToShortDateString()}";
            }

            return null;
        }

    }
}
