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
        var data = JsonDocument.Parse(json);

        var firstItem = data.RootElement.GetProperty("items").EnumerateArray().FirstOrDefault();
        if (firstItem.ValueKind != JsonValueKind.Undefined && firstItem.TryGetProperty("discount_percent", out var discount))
        {
            return discount.GetInt32() > 0
                ? $"{discount.GetInt32()}% off"
                : "No discount";
        }

        return null;
    }
}