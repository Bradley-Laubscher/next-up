using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextUp.Models
{
    public class Game
    {
        public int Id { get; set; }

        [Required]
        public required string Title { get; set; }

        public required string CoverImageUrl { get; set; }

        public required string Platform { get; set; }

        public required string Genre { get; set; }

        public required string Description { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public GameStatus? Status { get; set; }

        [Range(1, 10)]
        public int? Rating { get; set; }

        public string? Review { get; set; }

        // Foreign key to Identity user
        public required string UserId { get; set; }

        [ForeignKey("UserId")]
        public required ApplicationUser User { get; set; }
    }

    public enum GameStatus
    {
        Wishlist,
        Playing,
        Completed,
        Abandoned
    }
}