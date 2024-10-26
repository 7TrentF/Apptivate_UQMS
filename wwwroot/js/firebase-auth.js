// In wwwroot/js/firebase-auth.js

const firebaseConfig = {
    apiKey: "AIzaSyDXB0dEozyPQdXzlGyQFP1elMObEksarR0",
    authDomain: "uqms-a3d87.firebaseapp.com",
    projectId: "uqms-a3d87",
    storageBucket: "uqms-a3d87.appspot.com",
    messagingSenderId: "728626322581",
    appId: "1:728626322581:web:a5809843d30cf49aef4f9c",
    measurementId: "G-ZGLBS2PFY8"
};

// Initialize Firebase
firebase.initializeApp(firebaseConfig);

// Google Auth Provider
const provider = new firebase.auth.GoogleAuthProvider();

// Sign in function
async function signInWithGoogle() {
    try {
        const result = await firebase.auth().signInWithPopup(provider);
        const user = result.user;

        // Get the ID token
        const idToken = await user.getIdToken();

        // Send the token to your backend
        await fetch('/Account/ExternalLoginCallback', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                idToken: idToken,
                provider: 'Google'
            })
        });

        // Redirect to home page or dashboard
        window.location.href = '/';
    } catch (error) {
        console.error('Error during sign in:', error);
        document.getElementById('error-message').textContent = 'Failed to sign in with Google';
    }
}

// Auth state observer
firebase.auth().onAuthStateChanged((user) => {
    if (user) {
        // User is signed in
        console.log('User is signed in:', user.email);
    } else {
        // User is signed out
        console.log('User is signed out');
    }
});