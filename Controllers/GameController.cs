using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using NextUp.Data;
using NextUp.Models;
using Microsoft.AspNetCore.Identity;
using NextUp.Services;

namespace NextUp.Controllers
{
    [Authorize]
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SteamService _steamService;
        private readonly IgdbService _igdbService;

        public GamesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SteamService steamService, IgdbService igdbService)
        {
            _context = context;
            _userManager = userManager;
            _steamService = steamService;
            _igdbService = igdbService;
        }

        // GET: My List
        public async Task<IActionResult> MyList()
        {
            var userId = _context.Users.First(u => u.UserName == User.Identity.Name).Id;
            var userGames = await _context.Games
                .Where(g => g.UserId == userId)
                .ToListAsync();

            // Check for discounts using Steam API
            var tasks = userGames.Select(async game =>
            {
                game.SteamDiscountInfo = await _steamService.GetSteamDiscountInfo(game.Title) ?? "";
                game.UpcomingExpansionInfo = await _igdbService.GetUpcomingUpdateInfoAsync(game.Title);
            });
            await Task.WhenAll(tasks);
            await _context.SaveChangesAsync();

            return View(userGames);
        }

        // POST: Add a game from IGDB to "My List"
        [HttpPost]
        public async Task<IActionResult> AddToList(Game game)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            var userId = user.Id;
            game.UserId = userId;

            // Render requires release date to be in UTC
            if (game.ReleaseDate.HasValue)
            {
                var date = game.ReleaseDate.Value;
                game.ReleaseDate = date.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(date, DateTimeKind.Utc)
                    : date.ToUniversalTime();
            }

            // Check if this game already exists in the user's list
            bool gameExists = await _context.Games.AnyAsync(g =>
                g.Title == game.Title &&
                g.Platform == game.Platform &&
                g.UserId == userId
            );

            if (gameExists)
            {
                TempData["Message"] = "This game is already in your list.";
                return RedirectToAction("Index", "Home");
            }

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"{game.Title} has been added to your list.";
            return RedirectToAction("MyList", "Games");
        }

        // POST: Remove a game from "My List"
        [HttpPost]
        public async Task<IActionResult> RemoveFromList(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MyList");
        }
    }
}