using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FriendsApp2.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FriendsApp2.Api.Data
{
    public class FriendsRepository : IFriendsRepository
    {
        private readonly DataContext _context;
        public FriendsRepository(DataContext context)
        {
            _context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }
        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }


        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(k => k.Photos).FirstOrDefaultAsync(k => k.Id == id);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _context.Users.Include(k => k.Photos).ToListAsync();
            return users;
        }


        public async Task<bool> SaveAll()
        {

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            return await _context.Photos.FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId)
                                .FirstOrDefaultAsync(p => p.IsMain);
        }
    }
}