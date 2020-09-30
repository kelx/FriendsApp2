using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FriendsApp2.Api.helpers;
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

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(k => k.Photos)
                        .OrderByDescending(k => k.LastActive).AsQueryable(); ;

            users = users.Where(k => k.Id != userParams.UserId);
            if (!string.IsNullOrEmpty(userParams.Gender))
            {
                users = users.Where(k => k.Gender == userParams.Gender);
            }

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge - 1);

                users = users.Where(k => k.DateOfBirth >= minDob && k.DateOfBirth <= maxDob);
            }
            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(k => k.Created);
                        break;
                        
                    default:
                        users = users.OrderByDescending(k => k.LastActive);
                        break;
                }
            }


            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
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