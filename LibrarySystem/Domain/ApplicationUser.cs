using Microsoft.AspNetCore.Identity;

namespace LibrarySystem.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}