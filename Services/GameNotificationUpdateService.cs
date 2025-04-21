using Microsoft.EntityFrameworkCore;
using NextUp.Data;
using NextUp.Services;

public class GameUpdateNotifierService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public GameUpdateNotifierService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Run daily
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var steamService = scope.ServiceProvider.GetRequiredService<SteamService>();
            var igdbService = scope.ServiceProvider.GetRequiredService<IgdbService>();

            var games = dbContext.Games.Include(g => g.User).ToList();

            foreach (var game in games)
            {
                var currentDiscount = await steamService.GetSteamDiscountInfo(game.Title);
                var currentExpansion = await igdbService.GetUpcomingUpdateInfoAsync(game.Title);

                // 🔔 Check for discount change
                if (!string.IsNullOrWhiteSpace(currentDiscount) &&
                    currentDiscount != "No discount available" &&
                    currentDiscount != game.LastNotifiedDiscount)
                {
                    await emailService.SendDiscountNotificationAsync(game.User.Email, game.Title, currentDiscount);
                    game.LastNotifiedDiscount = currentDiscount;
                }

                // 🔔 Check for expansion change
                if (!string.IsNullOrWhiteSpace(currentExpansion) &&
                    currentExpansion != game.LastNotifiedExpansion)
                {
                    await emailService.SendExpansionNotificationAsync(game.User.Email, game.Title, currentExpansion);
                    game.LastNotifiedExpansion = currentExpansion;
                }

                game.SteamDiscountInfo = currentDiscount;
                game.UpcomingExpansionInfo = currentExpansion;
            }

            await dbContext.SaveChangesAsync();

            // Wait for 24 hours
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
