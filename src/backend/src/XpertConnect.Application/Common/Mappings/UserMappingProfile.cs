using AutoMapper;
using XpertConnect.Application.Features.Users.DTOs;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Application.Common.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserResponse>();
        CreateMap<User, UserListResponse>();

        CreateMap<UpdateUserRequest, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
