using Microsoft.AspNetCore.Identity;

namespace NextUp.Models
{
	// Inherits from IdentityUser to give you all the built-in identity features
	public class ApplicationUser : IdentityUser
	{
        public ICollection<Game> Games { get; set; }
    }
}
