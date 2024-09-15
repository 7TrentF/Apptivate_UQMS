using Apptivate_UQMS_WebApp.Data;
using Apptivate_UQMS_WebApp.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// Add authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Firebase";
    options.DefaultChallengeScheme = "Firebase";
})
.AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>("Firebase", null);

// Configure logging
builder.Logging.ClearProviders(); // Optional: Clears default providers
builder.Logging.AddConsole(); // Add console logging
builder.Logging.AddDebug(); // Add debug logging (visible in Debug output window)

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Build the app
var app = builder.Build();

// Enable session middleware
app.UseSession();

// Other middleware (e.g., authentication, routing)
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
