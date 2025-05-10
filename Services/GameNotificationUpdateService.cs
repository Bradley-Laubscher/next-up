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
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var firestore = scope.ServiceProvider.GetRequiredService<FirestoreService>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var steamService = scope.ServiceProvider.GetRequiredService<SteamService>();
            var igdbService = scope.ServiceProvider.GetRequiredService<IgdbService>();

            // ✅ Pull games from Firestore
            // You’ll need to loop over all users (or restructure Firestore to support efficient queries)
            var allUsers = await firestore.GetAllUsersAsync(); // you need to create this
            foreach (var user in allUsers)
            {
                var userGames = await firestore.GetUserGamesAsync(user.UserId);
                foreach (var game in userGames)
                {
                    var currentDiscount = await steamService.GetSteamDiscountInfo(game.Title);
                    var currentExpansion = await igdbService.GetUpcomingUpdateInfoAsync(game.Title);

                    if (!string.IsNullOrWhiteSpace(currentDiscount) &&
                        currentDiscount != "No discount available" &&
                        currentDiscount != game.LastNotifiedDiscount)
                    {
                        await emailService.SendDiscountNotificationAsync(user.Email, game.Title, currentDiscount);
                        game.LastNotifiedDiscount = currentDiscount;
                    }

                    if (!string.IsNullOrWhiteSpace(currentExpansion) &&
                        currentExpansion != game.LastNotifiedExpansion)
                    {
                        await emailService.SendExpansionNotificationAsync(user.Email, game.Title, currentExpansion);
                        game.LastNotifiedExpansion = currentExpansion;
                    }

                    game.SteamDiscountInfo = currentDiscount;
                    game.UpcomingExpansionInfo = currentExpansion;

                    // ✅ Save updated game
                    await firestore.UpdateUserGameAsync(game);
                }
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}