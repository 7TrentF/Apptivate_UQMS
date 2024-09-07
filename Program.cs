using Apptivate_UQMS_WebApp.Data;
using Apptivate_UQMS_WebApp.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Firebase initialization
var firebaseConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "Properties", "uqms-firebase-adminsdk.json");
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(firebaseConfigPath),
});

// Register FirebaseAuthService 
builder.Services.AddScoped<FirebaseAuthService>();

builder.Services.AddHttpClient<FirebaseAuthService>();

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

//builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();







// Build the app
var app = builder.Build();

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

