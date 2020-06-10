using Microsoft.EntityFrameworkCore;
using FriendsApp2.Api.Models;

namespace FriendsApp2.Api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options){}
        
        public DbSet<WeatherForecast> Weatherforcasts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        //public DbSet<Group> Groups { get; set; }
    }
}