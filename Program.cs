using DotNetEnv;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using NextUp.Services;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
Env.Load();
// This ensures environment variables override appsettings
builder.Configuration.AddEnvironmentVariables();
var config = builder.Configuration;

// Get sensitive data directly from environment variables
var firebaseKeyJson = config["FIREBASE_KEY_JSON"];
if (string.IsNullOrEmpty(firebaseKeyJson))
    throw new Exception("FIREBASE_KEY_JSON is not set in environment variables.");

var projectId = config["FIREBASE_PROJECT_ID"];
if (string.IsNullOrEmpty(projectId))
    throw new Exception("FIREBASE_PROJECT_ID is not set in environment variables.");

// Initialize Firebase app with the credential from environment variable
var credential = GoogleCredential.FromJson(firebaseKeyJson);

// Initialize Firebase app with the credential
FirebaseApp.Create(new AppOptions
{
    Credential = credential
});

builder.Services.AddAuthentication("Firebase")
    .AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>("Firebase", options => { });

builder.Services.AddAuthorization();

// Register services
builder.Services.AddSingleton(sp =>
    new FirestoreService(projectId!, credential)
);
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<IgdbAuthService>();
builder.Services.AddHttpClient<IgdbService>();
builder.Services.AddHttpClient<SteamService>();
builder.Services.AddHostedService<GameUpdateNotifierService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseMiddleware<FirebaseAuthMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();