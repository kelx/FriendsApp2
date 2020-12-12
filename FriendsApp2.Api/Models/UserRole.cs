using Microsoft.AspNetCore.Identity;

namespace FriendsApp2.Api.Models
{
    public class UserRole : IdentityUserRole<int>
    {
        public User User { get; set; }
        public Role Role { get; set; }
    }
}