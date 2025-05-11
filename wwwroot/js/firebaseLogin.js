window.addEventListener('DOMContentLoaded', () => {
    const firebaseConfig = {
        apiKey: "AIzaSyDwbCt9HaXqdIPQVp27O3FnASFpDgX34M8",
        authDomain: "nextup-8e151.firebaseapp.com",
        projectId: "nextup-8e151",
        storageBucket: "nextup-8e151.appspot.com",
        messagingSenderId: "814715303388",
        appId: "1:814715303388:web:34819e1aec042c0d9fcb19",
        measurementId: "G-2J50EYTFH3"
    };

    if (!firebase.apps.length) {
        firebase.initializeApp(firebaseConfig);
    }

    const auth = firebase.auth();

    let isSignUp = false;

    const toggleMode = () => {
        isSignUp = !isSignUp;
        document.getElementById('formTitle').innerText = isSignUp ? 'Sign Up' : 'Sign In';
        document.getElementById('submitButton').innerText = isSignUp ? 'Sign Up' : 'Sign In';
        document.getElementById('toggleLink').innerHTML = isSignUp
            ? `Already have an account? <a href="#" id="toggleMode">Sign In</a>`
            : `Don't have an account? <a href="#" id="toggleMode">Sign Up</a>`;

        document.getElementById('toggleMode').addEventListener('click', (e) => {
            e.preventDefault();
            toggleMode();
        });
    };

    document.getElementById('toggleMode').addEventListener('click', (e) => {
        e.preventDefault();
        toggleMode();
    });

    document.getElementById('submitButton').addEventListener('click', (e) => {
        e.preventDefault(); // Prevent form submission default behavior

        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;

        if (!email || !password) {
            alert("Email and password are required.");
            return;
        }

        if (isSignUp) {
            auth.createUserWithEmailAndPassword(email, password)
                .then(userCredential => handleAuthSuccess(userCredential.user))
                .catch(error => alert(error.message));
        } else {
            auth.signInWithEmailAndPassword(email, password)
                .then(userCredential => handleAuthSuccess(userCredential.user))
                .catch(error => alert(error.message));
        }
    });

    const handleAuthSuccess = async (user) => {
        try {
            const token = await user.getIdToken(true);
            document.cookie = `token=${token}; path=/;`;
            window.location.href = '/'; // Redirect only once upon success
        } catch (err) {
            console.error("Token retrieval failed:", err);
            alert("Authentication failed.");
        }
    };

    // Logout method if needed elsewhere
    window.logout = () => auth.signOut();
});