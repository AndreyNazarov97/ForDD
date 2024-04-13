using AutoMapper;
using ForDD.Domain.Dto.Role;
using ForDD.Domain.Entity;

namespace ForDD.Application.Mapping
{
    public class RoleMapping : Profile
    {
        public RoleMapping()
        {
            CreateMap<Role, RoleDto>()
              .ReverseMap();
        }
    }
}
