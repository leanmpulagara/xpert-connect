using AutoMapper;
using XpertConnect.Application.Features.Consultations.DTOs;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Application.Common.Mappings;

public class ConsultationMappingProfile : Profile
{
    public ConsultationMappingProfile()
    {
        // Consultation -> ConsultationResponse
        CreateMap<Consultation, ConsultationResponse>()
            .ForMember(dest => dest.ExpertName, opt => opt.MapFrom(src =>
                $"{src.Expert.User.FirstName} {src.Expert.User.LastName}"))
            .ForMember(dest => dest.ExpertProfilePhotoUrl, opt => opt.MapFrom(src =>
                src.Expert.User.ProfilePhotoUrl))
            .ForMember(dest => dest.SeekerName, opt => opt.MapFrom(src =>
                $"{src.Seeker.User.FirstName} {src.Seeker.User.LastName}"))
            .ForMember(dest => dest.SeekerProfilePhotoUrl, opt => opt.MapFrom(src =>
                src.Seeker.User.ProfilePhotoUrl))
            .ForMember(dest => dest.HasFeedback, opt => opt.MapFrom(src =>
                src.Feedback != null));

        // Consultation -> ConsultationListResponse
        CreateMap<Consultation, ConsultationListResponse>()
            .ForMember(dest => dest.ExpertName, opt => opt.MapFrom(src =>
                $"{src.Expert.User.FirstName} {src.Expert.User.LastName}"))
            .ForMember(dest => dest.SeekerName, opt => opt.MapFrom(src =>
                $"{src.Seeker.User.FirstName} {src.Seeker.User.LastName}"));
    }
}
