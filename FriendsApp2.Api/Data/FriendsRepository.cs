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


        public async Task<User> GetUser(int id, bool isCurrentUser)
        {
            var query = _context.Users.Include(p => p.Photos).AsQueryable();

            if (isCurrentUser)
                query = query.IgnoreQueryFilters();

            var user = await query.FirstOrDefaultAsync(k => k.Id == id);
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

            if (userParams.Likers)
            {
                var userLikers = await GetUserLike(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }
            if (userParams.Likees)
            {
                var userLikees = await GetUserLike(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));
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
        private async Task<IEnumerable<int>> GetUserLike(int id, bool likers)
        {
            var user = await _context.Users
                            .Include(k => k.Likers)
                            .Include(k => k.Likees)
                            .FirstOrDefaultAsync(k => k.Id == id);

            if (likers)
            {
                return user.Likers.Where(k => k.LikeeId == id).Select(k => k.LikerId);
            }
            else
            {
                return user.Likees.Where(k => k.LikerId == id).Select(k => k.LikeeId);
            }
        }


        public async Task<bool> SaveAll()
        {

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            return await _context.Photos.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId)
                                .FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recipientId);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages.Include(k => k.Sender).ThenInclude(k => k.Photos)
                                            .Include(k => k.Recipient).ThenInclude(k => k.Photos)
                                            .AsQueryable();

            switch (messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(k => k.RecipientId == messageParams.UserId
                        && k.RecipientDeleted == false);
                    break;
                case "Outbox":
                    messages = messages.Where(k => k.SenderId == messageParams.UserId
                        && k.SenderDeleted == false);
                    break;
                default:
                    messages = messages.Where(k => k.RecipientId == messageParams.UserId
                            && k.RecipientDeleted == false && k.IsRead == false);
                    break;
            }

            messages = messages.OrderByDescending(k => k.MessageSent);

            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber,
                                                        messageParams.PageSize);

        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await _context.Messages.Include(k => k.Sender).ThenInclude(k => k.Photos)
                                            .Include(k => k.Recipient).ThenInclude(k => k.Photos)
                                            .Where(k => k.RecipientId == userId && k.RecipientDeleted == false
                                                && k.SenderId == recipientId
                                                    ||
                                                    k.SenderId == userId && k.SenderDeleted == false
                                                && k.RecipientId == recipientId)
                                            .OrderByDescending(k => k.MessageSent).ToListAsync();
            return messages;
        }
    }
}