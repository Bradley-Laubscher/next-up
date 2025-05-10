using Google.Cloud.Firestore;

namespace NextUp.Models
{
    [FirestoreData]
    public class FirestoreUser
    {
        [FirestoreProperty]
        public string UserId { get; set; }  // Firebase UID or custom ID

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Email { get; set; }

        [FirestoreProperty]
        public string? ProfilePictureUrl { get; set; }
    }
}
