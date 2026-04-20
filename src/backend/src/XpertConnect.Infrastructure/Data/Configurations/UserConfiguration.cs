using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(u => u.Phone)
            .HasMaxLength(20);

        builder.Property(u => u.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.ProfilePhotoUrl)
            .HasMaxLength(1000);

        // Relationship: User -> Expert (one-to-one, optional)
        builder.HasOne(u => u.Expert)
            .WithOne(e => e.User)
            .HasForeignKey<Expert>(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship: User -> Seeker (one-to-one, optional)
        builder.HasOne(u => u.Seeker)
            .WithOne(s => s.User)
            .HasForeignKey<Seeker>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship: User -> NonProfitOrg (one-to-one, optional)
        builder.HasOne(u => u.NonProfitOrg)
            .WithOne(n => n.User)
            .HasForeignKey<NonProfitOrg>(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
