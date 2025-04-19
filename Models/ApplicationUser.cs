using Microsoft.AspNetCore.Identity;

namespace GameBacklogManager.Models
{
	// Inherits from IdentityUser to give you all the built-in identity features
	public class ApplicationUser : IdentityUser
	{
		// You can add extra properties to store additional user info

		// Example: public string? DisplayName { get; set; }

		// Keep it empty for now unless you need something specific
	}
}
