using System.ComponentModel.DataAnnotations;

namespace FriendsApp2.Api.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(8, MinimumLength=4, ErrorMessage= "You must specify password between 4 and 8 characotrs")]
        public string Password { get; set; }
    }
}