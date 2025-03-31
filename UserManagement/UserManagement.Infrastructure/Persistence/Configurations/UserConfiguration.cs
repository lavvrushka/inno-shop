using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Domain.Models;

namespace UserManagement.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(u => u.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(u => u.LastName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(u => u.Password)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(u => u.CreatedAt)
                .HasDefaultValueSql("NOW()");

            builder.Property(u => u.EmailVerifiedAt)
                .IsRequired(false);

            builder.Property(u => u.EmailConfirmationToken)
                .IsRequired(false)
                .HasMaxLength(255);

            builder.Property(u => u.PasswordResetToken)
                .IsRequired(false)
                .HasMaxLength(255);

            builder.Property(u => u.PasswordResetTokenExpires)
                .IsRequired(false);

            builder.Property(u => u.BirthDate)
                .IsRequired();
            builder.ToTable("Users");
        }
    }
}