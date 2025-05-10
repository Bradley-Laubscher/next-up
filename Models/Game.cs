using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextUp.Models
{
    [FirestoreData]
    public class Game
    {
        [FirestoreProperty]
        public int Id { get; set; }

        [FirestoreProperty]
        public string Title { get; set; } = string.Empty;

        [FirestoreProperty]
        public string CoverImageUrl { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Platform { get; set; } = string.Empty;

        [FirestoreProperty]
        public string? Genre { get; set; }

        [FirestoreProperty]
        public string? Description { get; set; }

        [FirestoreProperty]
        public DateTime? ReleaseDate { get; set; }

        [FirestoreProperty]
        public string? UpcomingExpansionInfo { get; set; }

        [FirestoreProperty]
        public string? SteamDiscountInfo { get; set; }

        [FirestoreProperty]
        public string? LastNotifiedDiscount { get; set; }

        [FirestoreProperty]
        public string? LastNotifiedExpansion { get; set; }

        [FirestoreProperty]
        public string UserId { get; set; } = string.Empty;

        [FirestoreProperty]
        public ApplicationUser User { get; set; } = default!;
    }
}