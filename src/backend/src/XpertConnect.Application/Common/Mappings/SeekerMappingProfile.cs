using AutoMapper;
using XpertConnect.Application.Features.Seekers.DTOs;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Application.Common.Mappings;

public class SeekerMappingProfile : Profile
{
    public SeekerMappingProfile()
    {
        // Seeker -> SeekerResponse
        CreateMap<Seeker, SeekerResponse>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
            .ForMember(dest => dest.ProfilePhotoUrl, opt => opt.MapFrom(src => src.User.ProfilePhotoUrl))
            .ForMember(dest => dest.VerificationStatus, opt => opt.MapFrom(src => src.User.VerificationStatus));
    }
}
