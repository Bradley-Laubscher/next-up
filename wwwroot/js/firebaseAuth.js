document.addEventListener("DOMContentLoaded", () => {
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

    // Handles logout
    window.logout = function () {
        auth.signOut().then(() => {
            location.href = '/Account/Login';
        });
    };

    auth.onAuthStateChanged((user) => {
        const welcomeMsg = document.getElementById('welcomeMsg');
        if (user) {
            document.getElementById('loginButton').classList.add('d-none');
            document.getElementById('logoutButton').classList.remove('d-none');
            welcomeMsg.classList.remove('d-none');
            welcomeMsg.innerHTML = `Welcome, ${user.email}`;
            document.getElementById('myListLink').classList.remove('d-none');
        } else {
            document.getElementById('loginButton').classList.remove('d-none');
            document.getElementById('logoutButton').classList.add('d-none');
            welcomeMsg.classList.add('d-none');
            document.getElementById('myListLink').classList.add('d-none');
        }
    });
});