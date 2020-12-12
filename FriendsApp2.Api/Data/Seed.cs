using System.Collections.Generic;
using System.Linq;
using FriendsApp2.Api.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace FriendsApp2.Api.Data
{
    public class Seed
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public Seed(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void SeedUsers()
        {

            var userData = System.IO.File.ReadAllText("Data/UserSeedData.Json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);
            var roles = new List<Role>
            {
                new Role {Name = "Member"},
                new Role {Name = "Admin"},
                new Role {Name = "SuperAdmin"},
                new Role {Name = "Moderator"}
            };
            foreach (var role in roles)
            {
                _roleManager.CreateAsync(role).Wait();
            }

            foreach (var user in users)
            {
                user.Photos.SingleOrDefault().IsApproved = true;
                _userManager.CreateAsync(user, "password").Wait();
                _userManager.AddToRoleAsync(user, "Member").Wait();
            }

            var kel = _userManager.FindByNameAsync("Kelmen").Result;
            _userManager.AddToRolesAsync(kel, new[] { "Admin", "SuperAdmin", "Moderator" }).Wait();

        }

    }
}