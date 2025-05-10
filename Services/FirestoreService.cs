using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using NextUp.Models;

namespace NextUp.Services
{
    public class FirestoreService
    {
        private readonly FirestoreDb _firestoreDb;

        public FirestoreService(string projectId, GoogleCredential credential)
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = credential
                });
            }

            var firestoreClient = new FirestoreClientBuilder
            {
                Credential = credential
            }.Build();

            _firestoreDb = FirestoreDb.Create(projectId, firestoreClient);
        }

        /// <summary>
        /// Verifies a Firebase ID token and returns the decoded token.
        /// </summary>
        public async Task<FirebaseToken> VerifyFirebaseTokenAsync(string idToken)
        {
            try
            {
                return await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("Firebase token verification failed.", ex);
            }
        }

        /// <summary>
        /// Retrieves all users from the Firestore 'users' collection.
        /// </summary>
        public async Task<List<FirestoreUser>> GetAllUsersAsync()
        {
            var usersRef = _firestoreDb.Collection("users");
            var snapshot = await usersRef.GetSnapshotAsync();

            return snapshot.Documents
                .Select(doc => doc.ConvertTo<FirestoreUser>())
                .ToList();
        }

        /// <summary>
        /// Updates an existing game in Firestore if it exists.
        /// </summary>
        public async Task UpdateUserGameAsync(Game game)
        {
            var gamesRef = _firestoreDb.Collection("games");
            var query = gamesRef
                .WhereEqualTo("UserId", game.UserId)
                .WhereEqualTo("Id", game.FirestoreId);

            var snapshot = await query.GetSnapshotAsync();
            var existingDoc = snapshot.Documents.FirstOrDefault();

            if (existingDoc != null)
            {
                await existingDoc.Reference.SetAsync(game);
            }
        }

        /// <summary>
        /// Retrieves all games for a specific user from Firestore.
        /// </summary>
        public async Task<List<Game>> GetUserGamesAsync(string userId)
        {
            var gamesRef = _firestoreDb.Collection("games");
            var query = gamesRef.WhereEqualTo("UserId", userId);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(doc =>
                {
                    var game = doc.ConvertTo<Game>();
                    game.FirestoreId = doc.Id;
                    return game;
                })
                .ToList();
        }

        /// <summary>
        /// Adds a new game document to Firestore.
        /// </summary>
        public async Task AddUserGameAsync(Game game)
        {
            var gameRef = _firestoreDb.Collection("games").Document();
            await gameRef.SetAsync(game);
        }

        /// <summary>
        /// Deletes a game from Firestore if it belongs to the specified user.
        /// </summary>
        public async Task DeleteUserGameAsync(string userId, string firestoreId)
        {
            var docRef = _firestoreDb.Collection("games").Document(firestoreId);
            var snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                var game = snapshot.ConvertTo<Game>();
                if (game.UserId == userId)
                {
                    await docRef.DeleteAsync();
                }
            }
        }
    }
}