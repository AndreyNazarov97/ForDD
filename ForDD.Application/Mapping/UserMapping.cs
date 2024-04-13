using AutoMapper;
using ForDD.Domain.Dto.User;
using ForDD.Domain.Entity;

namespace ForDD.Application.Mapping
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<User, UserDto>()
                .ReverseMap();
        }
    }
}
