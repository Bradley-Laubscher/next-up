using System.Text.Json;

public class SteamService
{
    private readonly HttpClient _httpClient;

    public SteamService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> GetSteamDiscountInfo(string gameName)
    {
        var response = await _httpClient.GetAsync($"https://store.steampowered.com/api/storesearch/?term={Uri.EscapeDataString(gameName)}&cc=us&l=en");

        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        using var data = JsonDocument.Parse(json);

        var firstItem = data.RootElement.GetProperty("items").EnumerateArray().FirstOrDefault();
        if (firstItem.ValueKind == JsonValueKind.Undefined) return "No discount available";

        if (firstItem.TryGetProperty("price", out var price))
        {
            var initial = price.GetProperty("initial").GetInt32();
            var final = price.GetProperty("final").GetInt32();

            if (initial > final)
            {
                var discountPercent = 100 - (final * 100 / initial);
                return $"{discountPercent}% off - ${final / 100.0:F2}";
            }
            else
            {
                return "No discount available";
            }
        }

        return "No discount available";
    }
}