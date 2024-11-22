function showSessionExpiredModal() {
    var modal = document.getElementById("sessionExpiredModal");
    modal.style.display = "block";

    document.getElementById("reloginButton").addEventListener("click", function () {
        window.location.href = "/Account/Login"; // Redirect to login page
    });
}

// Fetch example to handle 401 Unauthorized response
function checkForUnauthorized(response) {
    if (response.status === 401) {
        // Show modal if session expired or unauthorized
        showSessionExpiredModal();
    }
    return response.json();
}

// Example of an API call
fetch("/some/protected/endpoint")
    .then(response => checkForUnauthorized(response))
    .catch(error => console.error('Error:', error));
