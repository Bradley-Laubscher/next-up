using Microsoft.AspNetCore.Mvc;
using NextUp.Models;
using NextUp.Services;

namespace NextUp.Controllers;

public class HomeController : Controller
{
    private readonly IgdbService _igdbService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IgdbService igdbService, ILogger<HomeController> logger)
    {
        _igdbService = igdbService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new HomeViewModel()
        {
            NewReleases = new List<Game>(),
            ComingSoon = new List<Game>()
        };

        try
        {
            // Fetch upcoming games and new releases from IGDB API
            var upcomingGames = await _igdbService.FetchUpcomingGamesAsync();
            var newReleases = await _igdbService.FetchNewReleasesAsync();

            // Add them to the ViewModel
            viewModel.ComingSoon = upcomingGames;
            viewModel.NewReleases = newReleases;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while fetching IGDB data: " + ex.Message);
            // Handle error (optional)
        }

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Search(string searchQuery)
    {
        var viewModel = new HomeViewModel()
        {
            SearchResults = new List<Game>(),
            NewReleases = new List<Game>(),
            ComingSoon = new List<Game>()
        };

        var searchResults = await _igdbService.SearchGamesAsync(searchQuery);
        var newReleases = await _igdbService.FetchNewReleasesAsync();
        var comingSoon = await _igdbService.FetchUpcomingGamesAsync();

        viewModel.SearchResults = searchResults;
        viewModel.NewReleases = newReleases;
        viewModel.ComingSoon = comingSoon;

        return View("Index", viewModel);
    }
}