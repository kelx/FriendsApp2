using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FriendsApp2.Api.Data;
using FriendsApp2.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendsApp2.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IFriendsRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IFriendsRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();
            var usersToReturn = _mapper.Map<IEnumerable<UserForListsDto>>(users);
            return Ok(usersToReturn);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailedDto>(user);
            return Ok(userToReturn);
        }

    }
}