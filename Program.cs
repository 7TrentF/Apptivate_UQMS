using Apptivate_UQMS_WebApp.Data;
using Apptivate_UQMS_WebApp.Services;
using Apptivate_UQMS_WebApp.Services.AccountServices;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Serilog;
using Apptivate_UQMS_WebApp.Hubs;
using Apptivate_UQMS_WebApp.Services.QueryServices;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog to log to both the console and a file
builder.Host.UseSerilog((context, configuration) =>
    configuration
        .WriteTo.Console() // Logs to the console
        .WriteTo.File(
            Path.Combine(Directory.GetCurrentDirectory(), "Logs/log-.txt"), // Logs to the file
            rollingInterval: RollingInterval.Day));

// Add services to the container
builder.Services.AddControllersWithViews();
// Configure Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.EnableSensitiveDataLogging(); // Enable sensitive data logging
});


// Firebase initialization
var firebaseConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "Properties", "uqms-firebase-adminsdk.json");

if (!File.Exists(firebaseConfigPath))
{
    throw new FileNotFoundException("Firebase configuration file not found.", firebaseConfigPath);
}

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(firebaseConfigPath),
});

// Register services
builder.Services.AddTransient<FileUploadService>();
builder.Services.AddScoped<FirebaseAuthService>();
builder.Services.AddHttpClient<FirebaseAuthService>();
builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();
builder.Services.AddScoped<IQueryService, QuerySubmissionService>(); // Add this line
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IEmailService, BrevoEmailService>();
// In Startup.cs or Program.cs
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<AnalyticsService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ReportingService>();
// Add authentication

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Firebase";
    options.DefaultChallengeScheme = "Firebase";
})
.AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>("Firebase", null);

builder.Services.AddSignalR();


// Configure logging
builder.Logging.ClearProviders(); // Optional: Clears default providers
builder.Logging.AddConsole(); // Add console logging
builder.Logging.AddDebug(); // Add debug logging (visible in Debug output window)


builder.Logging.AddFile("Logs/log-{Date}.txt");  // Add this line to enable file logging


// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Account/AccessDenied";
});


// Build the app
var app = builder.Build();

// Enable session middleware
app.UseSession();

// Other middleware (e.g., authentication, routing)
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Map the SignalR Hub
app.MapHub<NotificationHub>("/notificationHub");

app.MapHub<ChatHub>("/chatHub");

app.MapControllers(); // Ensure controllers are mapped

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();


//app.UseHsts(); // Enforces HSTS policy
//app.UseXContentTypeOptions(); // Adds X-Content-Type-Options header
//app.UseReferrerPolicy(opts => opts.NoReferrer()); // Adds a referrer policy header


/*
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");
*/


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();