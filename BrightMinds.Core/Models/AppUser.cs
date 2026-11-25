using Microsoft.AspNetCore.Identity;

namespace BrightMinds.Core.Models
{
    public class AppUser:IdentityUser
    {
        public string FirstName { get; set; }   
        public string LastName { get; set; }
        public bool isDeleted { get; set; } = false;
    }
}
