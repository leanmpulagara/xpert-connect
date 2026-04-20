using AutoMapper;
using XpertConnect.Application.Features.TimeEntries.DTOs;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Application.Common.Mappings;

public class TimeEntryMappingProfile : Profile
{
    public TimeEntryMappingProfile()
    {
        // TimeEntry -> TimeEntryResponse
        CreateMap<TimeEntry, TimeEntryResponse>()
            .ForMember(dest => dest.ProjectTitle, opt => opt.MapFrom(src => src.Project.Title))
            .ForMember(dest => dest.ExpertName, opt => opt.MapFrom(src =>
                $"{src.Expert.User.FirstName} {src.Expert.User.LastName}"));
    }
}
