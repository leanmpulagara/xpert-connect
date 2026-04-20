using AutoMapper;
using XpertConnect.Application.Features.Experts.DTOs;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Application.Common.Mappings;

public class ExpertMappingProfile : Profile
{
    public ExpertMappingProfile()
    {
        // Expert -> ExpertResponse
        CreateMap<Expert, ExpertResponse>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
            .ForMember(dest => dest.ProfilePhotoUrl, opt => opt.MapFrom(src => src.User.ProfilePhotoUrl))
            .ForMember(dest => dest.VerificationStatus, opt => opt.MapFrom(src => src.User.VerificationStatus));

        // Expert -> ExpertListResponse
        CreateMap<Expert, ExpertListResponse>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.ProfilePhotoUrl, opt => opt.MapFrom(src => src.User.ProfilePhotoUrl))
            .ForMember(dest => dest.VerificationStatus, opt => opt.MapFrom(src => src.User.VerificationStatus))
            .ForMember(dest => dest.CredentialCount, opt => opt.MapFrom(src => src.Credentials.Count));

        // Credential -> CredentialResponse
        CreateMap<Credential, CredentialResponse>();

        // ExpertAvailability -> AvailabilityResponse
        CreateMap<ExpertAvailability, AvailabilityResponse>();
    }
}
