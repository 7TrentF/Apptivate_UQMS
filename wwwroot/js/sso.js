    console.log('Starting Firebase initialization...');

    const firebaseConfig = {
        apiKey: "AIzaSyDXB0dEozyPQdXzlGyQFP1elMObEksarR0",
    authDomain: "uqms-a3d87.firebaseapp.com",
    projectId: "uqms-a3d87",
    storageBucket: "uqms-a3d87.appspot.com",
    messagingSenderId: "728626322581",
    appId: "1:728626322581:web:a5809843d30cf49aef4f9c",
    measurementId: "G-ZGLBS2PFY8"
            };


    try {
                const app = firebase.initializeApp(firebaseConfig);
    console.log('Firebase initialized successfully');

    function initializeGoogleSignIn() {
        console.log('Setting up Google Sign-In...');
    const googleBtn = document.querySelector('.btn-google');

    if (!googleBtn) {
        console.error('Google sign-in button not found');
    return;
                    }

                    googleBtn.addEventListener('click', async (e) => {
        e.preventDefault();
    console.log('Google button clicked');

    try {
                            const provider = new firebase.auth.GoogleAuthProvider();
    const result = await firebase.auth().signInWithPopup(provider);
    const idToken = await result.user.getIdToken();

    const response = await fetch('/Account/GoogleLogin', {
        method: 'POST',
    headers: {
        'Content-Type': 'application/json',
    'Accept': 'application/json'  // Explicitly request JSON response
                                },
    body: JSON.stringify({idToken: idToken })
                            });

    // Check if response is JSON
    const contentType = response.headers.get('content-type');
    if (!contentType || !contentType.includes('application/json')) {
                                throw new Error('Received non-JSON response from server');
                            }

    const data = await response.json();
    console.log('Backend response:', data);

    if (data.success) {
        window.location.href = data.redirectUrl;
                            } else {
                                if (data.requiresRegistration) {
        alert('This email is not registered. Please register your account first with your university details.');
    window.location.href = '/Account/SelectRole';
                                } else {
        alert('Failed to login with Google: ' + data.error);
                                }
                            }
                        } catch (error) {
        console.error('Google sign-in error:', error);
    alert('Failed to login with Google: ' + error.message);
                        }
                    });
                }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializeGoogleSignIn);
                } else {
        initializeGoogleSignIn();
                }

            } catch (error) {
        console.error('Firebase initialization error:', error);
            }
