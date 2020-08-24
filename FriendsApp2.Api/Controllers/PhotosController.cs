using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FriendsApp2.Api.Data;
using FriendsApp2.Api.Dtos;
using FriendsApp2.Api.helpers;
using FriendsApp2.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FriendsApp2.Api.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IFriendsRepository _friendsRepo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IFriendsRepository friendsRepo, IMapper mapper,
                                IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _friendsRepo = friendsRepo;

            Account acc = new Account(
               _cloudinaryConfig.Value.CloudName,
               _cloudinaryConfig.Value.ApiKey,
               _cloudinaryConfig.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("{id}", Name = nameof(GetPhoto))]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _friendsRepo.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return Ok(photo);
        }
        // [HttpGet("{id}", Name = "GetPhoto")]
        // public async Task<IActionResult> GetPhoto(int id)
        // {
        //     var photoFromRepo = await _repo.GetPhoto(id);
        //     var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
        //     return Ok(photo);
        // }



        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoForCreationDto photoForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var userFromRepo = await _friendsRepo.GetUser(userId);

            var file = photoForCreationDto.File;
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().
                            Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            photoForCreationDto.Url = uploadResult.Url.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);

            if (!userFromRepo.Photos.Any(k => k.IsMain))
                photo.IsMain = true;

            userFromRepo.Photos.Add(photo);



            if (await _friendsRepo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                //return CreatedAtRoute("GetPhoto", new {id = photo.Id}, photoToReturn);
                //var crAtRt = CreatedAtRoute("GetPhoto", new {controller =  "photos", id = photo.Id }, photoToReturn);
                return CreatedAtRoute(nameof(GetPhoto), new { userId, id = photo.Id }, photoToReturn); ;
            }

            return BadRequest("Could not save photo.");

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var userFromRepo = await _friendsRepo.GetUser(userId);

            if (!userFromRepo.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await _friendsRepo.GetPhoto(id);

            if (photoFromRepo.IsMain)
                return BadRequest("This is already main photo.");

            var currentMainPhoto = await _friendsRepo.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if (await _friendsRepo.SaveAll())
                return NoContent();

            return BadRequest("Could not set photo to main.");

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {

            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var userFromRepo = await _friendsRepo.GetUser(userId);

            if (!userFromRepo.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await _friendsRepo.GetPhoto(id);

            if (photoFromRepo.IsMain)
                return BadRequest("You cannot delete your main photo.");

            if (photoFromRepo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deleteParams);
                if (result.Result == "ok")
                {
                    _friendsRepo.Delete(photoFromRepo);
                }
            }
            else
            {
                _friendsRepo.Delete(photoFromRepo);
            }

            if (await _friendsRepo.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Failed to delete the photo.");

        }
    }
}