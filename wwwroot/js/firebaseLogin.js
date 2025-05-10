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

    // Handle auth state
    firebase.auth().onAuthStateChanged(async user => {
        if (user) {
            // Force refresh to ensure we always have a fresh token
            const token = await user.getIdToken(true);
            console.log("Token length: ", token.length);
            document.cookie = `token=${token}; path=/;`;
            window.location.href = '/';
        }
    });

    // Toggle mode state
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

    // Handle login/signup
    document.getElementById('submitButton').addEventListener('click', () => {
        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;

        if (!email || !password) return alert("Email and password are required.");

        if (isSignUp) {
            auth.createUserWithEmailAndPassword(email, password)
                .catch(error => alert(error.message));
        } else {
            auth.signInWithEmailAndPassword(email, password)
                .catch(error => alert(error.message));
        }
    });

    // Logout
    window.logout = () => auth.signOut();
});