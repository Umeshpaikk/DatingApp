using System;
using System.Linq;
using API.DTO;
using API.Entities;
using API.Extentions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppUser, MemberDTO>().ForMember(dest => dest.Url, opt => opt.MapFrom( src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
                                           .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDTO>();

            CreateMap<MemberUpdateDTO, AppUser>();

            CreateMap<RegisterDTO, AppUser>();

            CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom( src =>
            src.Sender.Photos.FirstOrDefault(pic => pic.IsMain).Url))
            .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom( src =>
            src.Recipient.Photos.FirstOrDefault(pic => pic.IsMain).Url));
            CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d,DateTimeKind.Utc));
            
        }
    }
}