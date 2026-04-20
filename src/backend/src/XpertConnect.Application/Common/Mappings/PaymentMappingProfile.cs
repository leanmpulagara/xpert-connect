using AutoMapper;
using XpertConnect.Application.Features.Escrow.DTOs;
using XpertConnect.Application.Features.Payments.DTOs;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Application.Common.Mappings;

public class PaymentMappingProfile : Profile
{
    public PaymentMappingProfile()
    {
        // Payment -> PaymentResponse
        CreateMap<Payment, PaymentResponse>()
            .ForMember(dest => dest.ConsultationTitle, opt => opt.MapFrom(src =>
                src.Consultation != null
                    ? $"Consultation with {src.Consultation.Expert.User.FirstName} {src.Consultation.Expert.User.LastName}"
                    : null))
            .ForMember(dest => dest.HasEscrow, opt => opt.MapFrom(src => src.EscrowAccount != null))
            .ForMember(dest => dest.EscrowAccountId, opt => opt.MapFrom(src =>
                src.EscrowAccount != null ? src.EscrowAccount.Id : (Guid?)null));

        // EscrowAccount -> EscrowAccountResponse
        CreateMap<EscrowAccount, EscrowAccountResponse>()
            .ForMember(dest => dest.Milestones, opt => opt.MapFrom(src => src.Milestones));

        // Milestone -> MilestoneResponse
        CreateMap<Milestone, MilestoneResponse>();
    }
}
