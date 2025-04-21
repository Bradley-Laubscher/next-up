namespace NextUp.Services
{
    public interface IEmailService
    {
        Task SendDiscountNotificationAsync(string toEmail, string gameTitle, string discountInfo);
        Task SendExpansionNotificationAsync(string toEmail, string gameTitle, string expansionInfo);
    }
}
