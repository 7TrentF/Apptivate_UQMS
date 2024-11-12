namespace Apptivate_UQMS_WebApp.Services.AccountServices
{
    using FirebaseAdmin.Auth;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Security.Cryptography;
    using System.Text;

    public class FirebaseAuthService
    {
        private readonly ILogger<FirebaseAuthService> _logger;
        private readonly HttpClient _httpClient;

        public FirebaseAuthService(ILogger<FirebaseAuthService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        // Method to generate a hashed password using PBKDF2
        public string GeneratePasswordHash(string password)
        {
            // Generate a random salt
            using var rng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[16];
            rng.GetBytes(salt);

            // Hash the password with the salt using PBKDF2
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Combine salt and hash into one array for storage
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // Convert the combined salt+hash to a base64 string for storage
            return Convert.ToBase64String(hashBytes);
        }

        // Method to verify if a given password matches the stored hash
        public bool VerifyPasswordHash(string storedPasswordHash, string enteredPassword)
        {
            // Extract the bytes from the stored hash
            byte[] hashBytes = Convert.FromBase64String(storedPasswordHash);

            // Extract the salt from the stored hash
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Hash the entered password with the extracted salt
            using var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 100000);
            byte[] enteredHash = pbkdf2.GetBytes(20);

            // Compare the entered hash with the stored hash
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != enteredHash[i])
                {
                    return false; // Password does not match
                }
            }

            return true; // Password matches
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


        public async Task<string> LoginWithGoogle(string idToken)
        {
            _logger.LogInformation("Attempting to login with Google.");
            try
            {
                // Create the proper payload without URL encoding the token
                var payload = new
                {
                    postBody = $"id_token={idToken}&providerId=google.com",
                    requestUri = "http://localhost:5042",
                    returnSecureToken = true
                };

                var response = await _httpClient.PostAsJsonAsync(
                    "https://identitytoolkit.googleapis.com/v1/accounts:signInWithIdp?key=AIzaSyDXB0dEozyPQdXzlGyQFP1elMObEksarR0",
                    payload
                );

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received response from Firebase with Status Code: {StatusCode}", response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to login with Google. Status Code: {StatusCode}, Content: {Content}",
                        response.StatusCode, content);
                    throw new Exception($"Failed to login with Google: {content}");
                }

                var jsonDoc = JsonDocument.Parse(content);
                var firebaseToken = jsonDoc.RootElement.GetProperty("idToken").GetString();
                _logger.LogInformation("Google login successful, Firebase token retrieved.");
                return firebaseToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Google login.");
                throw;
            }
        }




        public async Task<string> RegisterUser(string email, string password, string role)
        {
            _logger.LogInformation($"Attempting to register user with email: {email}");

            try
            {
                // Check if a user with the email already exists
                var existingUser = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);
                if (existingUser != null)
                {
                    _logger.LogError($"A user with the email {email} already exists.");
                    throw new Exception("A user with this email already exists.");
                }
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.AuthErrorCode != AuthErrorCode.UserNotFound)
                {
                    _logger.LogError(ex, "Error while checking if the user already exists.");
                    throw new Exception("Error while checking user existence.");
                }
            }

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

                // Generate and send verification email
                var actionCodeSettings = new ActionCodeSettings()
                {
                    Url = "https://192.168.1.68:7086/Account/Login", // Redirect to your app after verification
                    HandleCodeInApp = true,
                };

                //

                var verificationLink = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(email, actionCodeSettings);

                // Optional: Send the link via Brevo
              //  await SendVerificationEmailWithBrevo(email, verificationLink);

                return userRecord.Uid;
            }
            catch (FirebaseAuthException ex)
            {
                var errorMessage = ex.Message.ToLower();

                if (errorMessage.Contains("email") && errorMessage.Contains("invalid"))
                {
                    _logger.LogError($"Invalid email format: {email}");
                    throw new Exception("Invalid email format.");
                }
                else if (errorMessage.Contains("password") && errorMessage.Contains("weak"))
                {
                    _logger.LogError("Password is too weak.");
                    throw new Exception("Password must be at least 6 characters long.");
                }
                else if (errorMessage.Contains("email") && errorMessage.Contains("already exists"))
                {
                    _logger.LogError($"Email address is already taken: {email}");
                    throw new Exception("Email address is already taken.");
                }
                else
                {
                    _logger.LogError(ex, $"Failed to register user with email: {email}");
                    throw new Exception("Registration failed. Please try again.");
                }
            }
        }

    }
}
