using AutoMapper;
using XpertConnect.Application.Features.Bids.DTOs;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Application.Common.Mappings;

public class BidMappingProfile : Profile
{
    public BidMappingProfile()
    {
        // Bid -> BidResponse
        CreateMap<Bid, BidResponse>()
            .ForMember(dest => dest.SeekerName, opt => opt.MapFrom(src =>
                $"{src.Seeker.User.FirstName} {src.Seeker.User.LastName}"));
    }
}
