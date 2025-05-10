using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NextUp.Models;
using NextUp.Services;
using System.Security.Claims;

namespace NextUp.Controllers
{
    [Authorize]
    public class GamesController : Controller
    {
        private readonly SteamService _steamService;
        private readonly IgdbService _igdbService;
        private readonly FirestoreService _firestore;

        public GamesController(
            SteamService steamService,
            IgdbService igdbService,
            FirestoreService firestore)
        {
            _steamService = steamService;
            _igdbService = igdbService;
            _firestore = firestore;
        }

        // Helper to get Firebase UID
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // GET: My List
        public async Task<IActionResult> MyList()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var userGames = await _firestore.GetUserGamesAsync(userId);

            var tasks = userGames.Select(async game =>
            {
                game.SteamDiscountInfo = await _steamService.GetSteamDiscountInfo(game.Title) ?? "";
                game.UpcomingExpansionInfo = await _igdbService.GetUpcomingUpdateInfoAsync(game.Title);
            });

            await Task.WhenAll(tasks);

            return View(userGames);
        }

        // POST: Add a game to "My List"
        [HttpPost]
        public async Task<IActionResult> AddToList(Game game)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            game.UserId = userId;

            if (string.IsNullOrEmpty(game.Platform))
                game.Platform = "Unknown";

            if (game.ReleaseDate.HasValue)
            {
                var date = game.ReleaseDate.Value;
                game.ReleaseDate = date.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(date, DateTimeKind.Utc)
                    : date.ToUniversalTime();
            }

            var existingGames = await _firestore.GetUserGamesAsync(userId);
            bool exists = existingGames.Any(g =>
                g.Title == game.Title && g.Platform == game.Platform);

            if (exists)
            {
                TempData["Message"] = "This game is already in your list.";
                return RedirectToAction("Index", "Home");
            }

            await _firestore.AddUserGameAsync(game);
            TempData["Message"] = $"{game.Title} has been added to your list.";
            return RedirectToAction("MyList");
        }

        // POST: Remove a game
        [HttpPost]
        public async Task<IActionResult> RemoveFromList(int id)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            await _firestore.DeleteUserGameAsync(userId, id);
            return RedirectToAction("MyList");
        }
    }
}