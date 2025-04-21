using NextUp.Services;
using System.Net.Mail;
using System.Net;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendDiscountNotificationAsync(string toEmail, string gameTitle, string discountInfo)
    {
        var subject = $"🔥 {gameTitle} is now on discount!";
        var body = $"Good news! {gameTitle} is now available at {discountInfo} on Steam. Check it out!";

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendExpansionNotificationAsync(string toEmail, string gameTitle, string expansionInfo)
    {
        var subject = $"📢 New Update for {gameTitle}!";
        var body = $"Heads up! {gameTitle} has a new expansion or update coming: {expansionInfo}.";

        await SendEmailAsync(toEmail, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var smtpHost = _config["EmailSettings:SmtpHost"];
        var smtpPort = int.Parse(_config["EmailSettings:SmtpPort"]);
        var smtpUser = _config["EmailSettings:SmtpUser"];
        var smtpPass = Environment.GetEnvironmentVariable("NEXT_UP_EMAIL_APP_PASSWORD");
        var fromEmail = _config["EmailSettings:FromEmail"];

        if (string.IsNullOrEmpty(smtpPass))
        {
            throw new InvalidOperationException("SMTP password is missing.");
        }

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true
        };

        var mail = new MailMessage(fromEmail, toEmail, subject, body);
        await client.SendMailAsync(mail);
    }
}