using CloudinaryDotNet;
using FriendsApp2.Api.Data;
using FriendsApp2.Api.Dtos;
using FriendsApp2.Api.helpers;
using FriendsApp2.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using CloudinaryDotNet.Actions;

namespace FriendsApp2.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private Cloudinary _cloudinary;
        private readonly UserManager<User> _userManager;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        public AdminController(DataContext context, UserManager<User> userManager,
                                IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _userManager = userManager;
            _context = context;
            _cloudinaryConfig = cloudinaryConfig;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        [Authorize(Policy = "RequireAdminRole", AuthenticationSchemes = "Bearer")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var userList = await (from user in _context.Users
                                  orderby user.UserName
                                  select new
                                  {
                                      Id = user.Id,
                                      UserName = user.UserName,
                                      BlockedUser = user.BlockedUser,
                                      Roles = (from userRole in user.UserRoles
                                               join role in _context.Roles
                                               on userRole.RoleId equals role.Id
                                               select role.Name).ToList()
                                  }).ToListAsync();

            return Ok(userList);
        }
        [Authorize(Policy = "ModeratePhotoRole", AuthenticationSchemes = "Bearer")]
        [HttpGet("photosForModeration")]
        public async Task<IActionResult> GetPhotosForModeration()
        {
            var photos = await _context.Photos
            .Include(k => k.User)
            .IgnoreQueryFilters()
            .Where(p => p.IsApproved == false)
            .Select(u => new
            {
                Id = u.Id,
                UserName = u.User.UserName,
                Url = u.Url,
                isApproved = u.IsApproved
            }).ToListAsync();

            return Ok(photos);
        }
        [Authorize(Policy = "ModeratePhotoRole", AuthenticationSchemes = "Bearer")]
        [HttpPost("approvePhoto/{photoId}")]
        public async Task<IActionResult> ApprovePhoto(int photoId)
        {
            var photo = await _context.Photos
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == photoId);
            photo.IsApproved = true;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Policy = "ModeratePhotoRole", AuthenticationSchemes = "Bearer")]
        [HttpPost("rejectPhoto/{photoId}")]
        public async Task<IActionResult> RejectPhoto(int photoId)
        {
            var photo = await _context.Photos
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == photoId);

            if (photo.IsMain)
                return BadRequest("You cannot reject the main photo.");

            if (photo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photo.PublicId);
                var result = _cloudinary.Destroy(deleteParams);

                if (result.Result == "ok")
                    _context.Photos.Remove(photo);
            }
            if (photo.PublicId == null)
            {
                _context.Photos.Remove(photo);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Policy = "RequireAdminRole", AuthenticationSchemes = "Bearer")]
        [HttpPost("editRoles/{userId}")]
        public async Task<IActionResult> EditRoles(int userId, RoleEditDto roleEditDto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var userRoles = await _userManager.GetRolesAsync(user);

            var selectedRoles = roleEditDto.RoleNames;

            // double  ?? null-coalescing operator
            // selected = selectedRoles != null ? selectedRoles : new string[] {}
            selectedRoles = selectedRoles ?? new string[] { };
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
                return BadRequest("Failed to add to rolls");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded)
                return BadRequest("Failed to remove roles.");

            return Ok(await _userManager.GetRolesAsync(user));

        }
        [Authorize(Policy = "RequireAdminRole", AuthenticationSchemes = "Bearer")]
        [HttpPost("blockUser/{userId}")]
        public async Task<IActionResult> BlockUser(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            user.BlockedUser = true;
            _context.Update(user);

            if (await _context.SaveChangesAsync() > 0)
                return NoContent();

            return BadRequest("Could not block user.");

        }
        [Authorize(Policy = "RequireAdminRole", AuthenticationSchemes = "Bearer")]
        [HttpPost("unBlockUser/{userId}")]
        public async Task<IActionResult> UnBlockUser(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            user.BlockedUser = false;
            _context.Update(user);

            if (await _context.SaveChangesAsync() > 0)
                return NoContent();

            return BadRequest("Could not Unblock user.");

        }

    }
}