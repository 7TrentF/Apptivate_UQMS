using Apptivate_UQMS_WebApp.Services;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

public class FirebaseAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly FirebaseAuthService _firebaseAuthService;

    public FirebaseAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        FirebaseAuthService firebaseAuthService)
        : base(options, logger, encoder, clock)
    {
        _firebaseAuthService = firebaseAuthService;
    }


    [Authorize]
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Cookies.TryGetValue("FirebaseToken", out var token))
        {
            // If no token is provided, return authentication failure
            return AuthenticateResult.Fail("No token provided.");
        }

        try
        {
            // Validate the ID token
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, decodedToken.Uid),
            new Claim(ClaimTypes.Name, decodedToken.Uid)
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
