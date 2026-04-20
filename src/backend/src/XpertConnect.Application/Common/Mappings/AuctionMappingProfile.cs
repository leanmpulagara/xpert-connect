using AutoMapper;
using XpertConnect.Application.Features.Auctions.DTOs;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Application.Common.Mappings;

public class AuctionMappingProfile : Profile
{
    public AuctionMappingProfile()
    {
        // AuctionLot -> AuctionResponse
        CreateMap<AuctionLot, AuctionResponse>()
            .ForMember(dest => dest.ExpertName, opt => opt.MapFrom(src =>
                $"{src.Expert.User.FirstName} {src.Expert.User.LastName}"))
            .ForMember(dest => dest.ExpertProfilePhotoUrl, opt => opt.MapFrom(src =>
                src.Expert.User.ProfilePhotoUrl))
            .ForMember(dest => dest.ExpertHeadline, opt => opt.MapFrom(src =>
                src.Expert.Headline))
            .ForMember(dest => dest.BeneficiaryOrgName, opt => opt.MapFrom(src =>
                src.BeneficiaryOrg != null ? src.BeneficiaryOrg.OrgName : null))
            .ForMember(dest => dest.WinnerName, opt => opt.MapFrom(src =>
                src.WinningBid != null
                    ? $"{src.WinningBid.Seeker.User.FirstName} {src.WinningBid.Seeker.User.LastName}"
                    : null))
            .ForMember(dest => dest.TotalBids, opt => opt.MapFrom(src =>
                src.Bids.Count));

        // AuctionLot -> AuctionListResponse
        CreateMap<AuctionLot, AuctionListResponse>()
            .ForMember(dest => dest.ExpertName, opt => opt.MapFrom(src =>
                $"{src.Expert.User.FirstName} {src.Expert.User.LastName}"))
            .ForMember(dest => dest.ExpertProfilePhotoUrl, opt => opt.MapFrom(src =>
                src.Expert.User.ProfilePhotoUrl))
            .ForMember(dest => dest.TotalBids, opt => opt.MapFrom(src =>
                src.Bids.Count));
    }
}
