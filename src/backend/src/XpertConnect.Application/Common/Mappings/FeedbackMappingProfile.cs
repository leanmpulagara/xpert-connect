using AutoMapper;
using XpertConnect.Application.Features.Feedback.DTOs;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Application.Common.Mappings;

public class FeedbackMappingProfile : Profile
{
    public FeedbackMappingProfile()
    {
        // Feedback -> FeedbackResponse
        CreateMap<Feedback, FeedbackResponse>()
            .ForMember(dest => dest.SeekerName, opt => opt.MapFrom(src =>
                $"{src.Seeker.User.FirstName} {src.Seeker.User.LastName}"));
    }
}
