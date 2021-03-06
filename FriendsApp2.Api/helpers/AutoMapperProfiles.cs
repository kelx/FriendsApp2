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
                dest => dest.PhotoUrl, opt =>
                {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(k => k.IsMain).Url);
                }
            ).ForMember(
                dest => dest.Age, opt =>
                {
                    opt.MapFrom((s, d) => s.DateOfBirth.CalculateAge());
                }
            );
            CreateMap<User, UserForDetailedDto>().ForMember(
                dest => dest.PhotoUrl, opt =>
                {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(k => k.IsMain).Url);
                }
            ).ForMember(
                dest => dest.Age, opt =>
                {
                    opt.MapFrom((s, d) => s.DateOfBirth.CalculateAge());
                }
            );
            CreateMap<Photo, PhotosForDetailedDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<Photo, PhotoForReturnDto>(); // source, destination
            CreateMap<UserForRegisterDto, User>();
            CreateMap<MessageForCreationDto, Message>().ReverseMap();
            CreateMap<Message, MessageToReturnDto>()
                .ForMember(k => k.SenderPhotoUrl, opt =>
                            opt.MapFrom(k => k.Sender.Photos.FirstOrDefault(k => k.IsMain).Url))
                .ForMember(k => k.RecipientPhotoUrl, opt =>
                            opt.MapFrom(k => k.Recipient.Photos.FirstOrDefault(k => k.IsMain).Url));

        }

    }
}