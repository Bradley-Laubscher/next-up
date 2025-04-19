using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextUp.Models
{
    public class Game
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string CoverImageUrl { get; set; }

        public string Platform { get; set; }

        public string Genre { get; set; }

        public string Description { get; set; }

        public GameStatus Status { get; set; }

        [Range(1, 10)]
        public int? Rating { get; set; }

        public string Review { get; set; }

        // Foreign key to Identity user
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }

    public enum GameStatus
    {
        Wishlist,
        Playing,
        Completed,
        Abandoned
    }
}