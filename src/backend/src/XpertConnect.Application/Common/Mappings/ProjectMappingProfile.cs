using AutoMapper;
using XpertConnect.Application.Features.Projects.DTOs;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Application.Common.Mappings;

public class ProjectMappingProfile : Profile
{
    public ProjectMappingProfile()
    {
        // ProBonoProject -> ProjectResponse
        CreateMap<ProBonoProject, ProjectResponse>()
            .ForMember(dest => dest.OrgName, opt => opt.MapFrom(src => src.Org.OrgName))
            .ForMember(dest => dest.OrgLogoUrl, opt => opt.Ignore())
            .ForMember(dest => dest.ExpertName, opt => opt.MapFrom(src =>
                src.Expert != null ? $"{src.Expert.User.FirstName} {src.Expert.User.LastName}" : null))
            .ForMember(dest => dest.ExpertProfilePhotoUrl, opt => opt.MapFrom(src =>
                src.Expert != null ? src.Expert.User.ProfilePhotoUrl : null))
            .ForMember(dest => dest.HasMou, opt => opt.MapFrom(src => src.Mou != null))
            .ForMember(dest => dest.MouSignedAt, opt => opt.MapFrom(src =>
                src.Mou != null ? src.Mou.SignedAt : null))
            .ForMember(dest => dest.TimeEntriesCount, opt => opt.MapFrom(src => src.TimeEntries.Count));

        // ProBonoProject -> ProjectListResponse
        CreateMap<ProBonoProject, ProjectListResponse>()
            .ForMember(dest => dest.OrgName, opt => opt.MapFrom(src => src.Org.OrgName))
            .ForMember(dest => dest.OrgLogoUrl, opt => opt.Ignore())
            .ForMember(dest => dest.ExpertName, opt => opt.MapFrom(src =>
                src.Expert != null ? $"{src.Expert.User.FirstName} {src.Expert.User.LastName}" : null));

        // Mou -> MouResponse
        CreateMap<Mou, MouResponse>()
            .ForMember(dest => dest.ProjectTitle, opt => opt.MapFrom(src => src.Project.Title));
    }
}
