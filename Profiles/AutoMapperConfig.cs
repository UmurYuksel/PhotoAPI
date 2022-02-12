using AutoMapper;
using PhotoAPI.DTO_s;
using PhotoAPI.DTO_s;
using PhotoAPI.Models;

namespace PhotoAPI.Profiles
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Photo, PhotoRequestDTO>().ReverseMap();
            CreateMap<Photo, PhotoResponseDTO>().ReverseMap();
        }
    }
}
