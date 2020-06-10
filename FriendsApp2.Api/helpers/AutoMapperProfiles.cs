using System.Linq;
using AutoMapper;
using FriendsApp2.Api.Dtos;
using FriendsApp2.Api.Models;

namespace FriendsApp2.Api.helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListsDto>().ForMember(
                dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(k => k.IsMain).Url);
                }
            ).ForMember(
                dest => dest.Age, opt => {
                    opt.MapFrom((s, d) => s.DateOfBirth.CalculateAge());
                }
            );
            CreateMap<User, UserForDetailedDto>().ForMember(
                dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(k => k.IsMain).Url);
                }
            ).ForMember(
                dest => dest.Age, opt => {
                    opt.MapFrom((s, d) => s.DateOfBirth.CalculateAge());
                }
            );
            CreateMap<Photo, PhotosForDetailedDto>();
        }
        
    }
}