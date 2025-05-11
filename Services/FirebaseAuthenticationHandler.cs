using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace NextUp.Services
{
    public class FirebaseAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public FirebaseAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock) { }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var token = Context.Request.Cookies["token"];

            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.Fail("No Firebase token found.");
            }

            try
            {
                var firebaseToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, firebaseToken.Uid),
                    new Claim(ClaimTypes.Name, firebaseToken.Claims.ContainsKey("name") ? firebaseToken.Claims["name"].ToString() : firebaseToken.Uid),
                    new Claim(ClaimTypes.Email, firebaseToken.Claims.ContainsKey("email") ? firebaseToken.Claims["email"].ToString() : "")
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail($"Firebase token validation failed: {ex.Message}");
            }
        }
    }
}
