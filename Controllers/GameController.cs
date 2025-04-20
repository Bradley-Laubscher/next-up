using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using NextUp.Data;
using NextUp.Models;
using Microsoft.AspNetCore.Identity;

namespace NextUp.Controllers
{
    [Authorize]
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GamesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: My List
        public async Task<IActionResult> Index()
        {
            var userId = _context.Users.First(u => u.UserName == User.Identity.Name).Id;
            var userGames = await _context.Games
                .Where(g => g.UserId == userId)
                .ToListAsync();

            return View(userGames);
        }

        // GET: Add a game manually
        public IActionResult Create()
        {
            return View();
        }

        // POST: Add a game manually
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Game game)
        {
            if (ModelState.IsValid)
            {
                var userId = _context.Users.First(u => u.UserName == User.Identity.Name).Id;
                game.UserId = userId;

                _context.Add(game);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(game);
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
            return RedirectToAction("Index", "Games");
        }
    }
}