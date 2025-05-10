using FirebaseAdmin;
using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;
using NextUp.Models;
using Google.Cloud.Firestore.V1;
using FirebaseAdmin.Auth;

namespace NextUp.Services
{
    public class FirestoreService
    {
        private readonly FirestoreDb _firestoreDb;

        public FirestoreService(string projectId, GoogleCredential credential)
        {
            // Initialize Firebase Firestore using the passed credential
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = credential
                });
            }

            // Build Firestore client using the given credential
            var firestoreClient = new FirestoreClientBuilder
            {
                Credential = credential
            }.Build();

            _firestoreDb = FirestoreDb.Create(projectId, firestoreClient);
        }

        // Method to verify Firebase ID token
        public async Task<FirebaseToken> VerifyFirebaseTokenAsync(string idToken)
        {
            try
            {
                // Verifying the Firebase ID token
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                return decodedToken; // This now returns a FirebaseToken
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("Firebase token verification failed", ex);
            }
        }
        public async Task<List<FirestoreUser>> GetAllUsersAsync()
        {
            var usersRef = _firestoreDb.Collection("users");
            var snapshot = await usersRef.GetSnapshotAsync();
            return snapshot.Documents.Select(d => d.ConvertTo<FirestoreUser>()).ToList();
        }

        public async Task UpdateUserGameAsync(Game game)
        {
            var gamesRef = _firestoreDb.Collection("games");
            var query = gamesRef.WhereEqualTo("UserId", game.UserId).WhereEqualTo("Id", game.Id);
            var snapshot = await query.GetSnapshotAsync();
            var doc = snapshot.Documents.FirstOrDefault();

            if (doc != null)
                await doc.Reference.SetAsync(game);
        }

        // Method to retrieve user games from Firestore
        public async Task<List<Game>> GetUserGamesAsync(string userId)
        {
            var gamesRef = _firestoreDb.Collection("games");
            var query = gamesRef.WhereEqualTo("UserId", userId);
            var snapshot = await query.GetSnapshotAsync();

            var userGames = new List<Game>();
            foreach (var document in snapshot.Documents)
            {
                var game = document.ConvertTo<Game>();
                userGames.Add(game);
            }

            return userGames;
        }

        // Method to add a game to the user's game list
        public async Task AddUserGameAsync(Game game)
        {
            var gameRef = _firestoreDb.Collection("games").Document();
            await gameRef.SetAsync(game);
        }

        // Method to delete a game from the user's game list
        public async Task DeleteUserGameAsync(string userId, int gameId)
        {
            var gamesRef = _firestoreDb.Collection("games");
            var query = gamesRef.WhereEqualTo("UserId", userId).WhereEqualTo("Id", gameId);
            var snapshot = await query.GetSnapshotAsync();
            var document = snapshot.Documents.FirstOrDefault();

            if (document != null)
            {
                await document.Reference.DeleteAsync();
            }
        }
    }
}