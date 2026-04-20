using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Common.Models;
using XpertConnect.Infrastructure.Data;
using XpertConnect.Infrastructure.Identity;
using XpertConnect.Infrastructure.Repositories;
using XpertConnect.Infrastructure.Services;

namespace XpertConnect.Infrastructure;

/// <summary>
/// Dependency injection configuration for Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // Identity
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // JWT Settings
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        // Services
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddScoped<ITokenService, TokenService>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IExpertRepository, ExpertRepository>();
        services.AddScoped<ISeekerRepository, SeekerRepository>();
        services.AddScoped<IConsultationRepository, ConsultationRepository>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        services.AddScoped<IAuctionRepository, AuctionRepository>();
        services.AddScoped<IBidRepository, BidRepository>();
        services.AddScoped<IProBonoProjectRepository, ProBonoProjectRepository>();
        services.AddScoped<ITimeEntryRepository, TimeEntryRepository>();
        services.AddScoped<INonProfitOrgRepository, NonProfitOrgRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IEscrowRepository, EscrowRepository>();

        // Payment Service (Mock for development, replace with StripePaymentService in production)
        services.AddScoped<IPaymentService, MockPaymentService>();

        // Email Service (Mock for development, replace with SendGridEmailService in production)
        services.AddScoped<IEmailService, MockEmailService>();

        // KYC Service (Mock for development, replace with OnfidoKycService in production)
        services.AddHttpClient("Onfido", client =>
        {
            client.BaseAddress = new Uri(configuration["Onfido:BaseUrl"] ?? "https://api.onfido.com/v3.6/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        services.AddScoped<IKycService, MockKycService>();

        // Geofence Service (Mock for development, replace with GeofenceService in production)
        services.AddScoped<IGeofenceService, MockGeofenceService>();

        return services;
    }
}
