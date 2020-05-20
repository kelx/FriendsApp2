using System.Threading.Tasks;
using FriendsApp2.Api.Models;

namespace FriendsApp2.Api.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(User user, string password);
         Task<User> Login(string username, string password);
         Task<bool> UserExists(string username);
    }
}