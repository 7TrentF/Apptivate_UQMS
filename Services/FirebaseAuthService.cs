namespace Apptivate_UQMS_WebApp.Services
{
    using FirebaseAdmin.Auth;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text.Json;

    public class FirebaseAuthService
    {
        private readonly ILogger<FirebaseAuthService> _logger;
        private readonly HttpClient _httpClient;

        public FirebaseAuthService(ILogger<FirebaseAuthService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task AssignUserRole(string userId, string role)
        {
            try
            {
                // Create a dictionary to hold the custom claims
                var claims = new Dictionary<string, object>
                {
                    { "role", role }
                };

                // Assign the custom claims to the user
                await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userId, claims);

                _logger.LogInformation($"Assigned role '{role}' to user with UID: {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to assign role '{role}' to user with UID: {userId}");
                throw;
            }
        }

        public async Task<string> RegisterUser(string email, string password, string role)
        {
            _logger.LogInformation($"Attempting to register user with email: {email}");

            var userRecordArgs = new UserRecordArgs
            {
                Email = email,
                EmailVerified = false,
                Password = password,
                Disabled = false,
            };

            try
            {
                // Create the user
                var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs);

                // Assign the role to the user
                await AssignUserRole(userRecord.Uid, role);

                _logger.LogInformation($"User registered successfully with UID: {userRecord.Uid}");
                return userRecord.Uid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to register user with email: {email}");
                throw;
            }
        }

        public async Task<string> LoginUser(string email, string password)
        {
            _logger.LogInformation($"Attempting to log in user with email: {email}");

            try
            {
                // Define the request payload
                var payload = new
                {
                    email,
                    password,
                    returnSecureToken = true
                };

                // Make the request to Firebase Authentication REST API
                var response = await _httpClient.PostAsJsonAsync($"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=AIzaSyDXB0dEozyPQdXzlGyQFP1elMObEksarR0", payload);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to log in user with email: {email}, StatusCode: {response.StatusCode}");
                    throw new Exception("Failed to log in user");
                }

                // Parse the response to get the ID token
                var content = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(content);
                var idToken = jsonDoc.RootElement.GetProperty("idToken").GetString();

                _logger.LogInformation("User logged in successfully.");
                return idToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to log in user with email: {email}");
                throw;
            }
        }
    }
}
