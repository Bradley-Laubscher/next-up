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

        public string? Genre { get; set; }

        public string? Description { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [NotMapped]
        public string? UpcomingExpansionInfo { get; set; }

        [NotMapped]
        public string? SteamDiscountInfo { get; set; }

        // Foreign key to Identity user
        public required string UserId { get; set; }

        [ForeignKey("UserId")]
        public required ApplicationUser User { get; set; }
    }
}