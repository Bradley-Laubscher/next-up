using NextUp.Services;
using System.Security.Claims;

public class FirebaseAuthMiddleware
{
    private readonly RequestDelegate _next;

    public FirebaseAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, FirestoreService firestoreService)
    {
        if (context.Request.Cookies.TryGetValue("token", out var idToken))
        {
            try
            {
                var decoded = await firestoreService.VerifyFirebaseTokenAsync(idToken);
                var claimsIdentity = new ClaimsIdentity("Firebase");
                claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, decoded.Uid));
                context.User = new ClaimsPrincipal(claimsIdentity);
            }
            catch
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid Firebase ID token.");
                return;
            }
        }

        await _next(context);
    }
}
