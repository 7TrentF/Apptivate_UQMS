function validateRegistrationForm() {
    var email = document.getElementById("email").value;
    var password = document.getElementById("password").value;
    var errorMessage = "";

    // Email validation
    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
        errorMessage += "Invalid email format.\n";
    }

    // Password validation
    if (password.length < 6) {
        errorMessage += "Password must be at least 6 characters long.\n";
    }

    if (errorMessage !== "") {
        alert(errorMessage);
        return false; // Prevent form submission
    }

    return true; // Allow form submission
}
