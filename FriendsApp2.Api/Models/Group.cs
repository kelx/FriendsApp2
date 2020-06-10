using System;

namespace FriendsApp2.Api.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public DateTime GroupCreated { get; set; }
    }
}