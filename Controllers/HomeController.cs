using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NextUp.Data;
using NextUp.Models;

namespace NextUp.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Index()
    {
        var currentDate = DateTime.Now;

        var viewModel = new HomeViewModel
        {
            NewReleases = _context.Games
                .Where(g => g.ReleaseDate != null && g.ReleaseDate <= currentDate)
                .OrderByDescending(g => g.ReleaseDate)
                .Take(5)
            .ToList(),

            ComingSoon = _context.Games
                .Where(g => g.ReleaseDate != null && g.ReleaseDate > currentDate)
                .OrderBy(g => g.ReleaseDate)
                .Take(5)
                .ToList()
        };

        return View(viewModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
