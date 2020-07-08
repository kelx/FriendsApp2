using System.Collections.Generic;
using System.Threading.Tasks;
using FriendsApp2.Api.Models;

namespace FriendsApp2.Api.Data
{
    public interface IFriendsRepository
    {
         void Add<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;
         void Update<T>(T entity) where T: class;
         Task<bool> SaveAll();
         Task<IEnumerable<User>> GetUsers();
         Task<User> GetUser(int id);
    }
}