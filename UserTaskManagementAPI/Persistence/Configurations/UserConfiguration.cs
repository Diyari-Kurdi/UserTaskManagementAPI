using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserTaskManagementAPI.Domain.Entities;
using UserTaskManagementAPI.Domain.Rules;

namespace UserTaskManagementAPI.Infrastructure.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(p => p.Username)
            .HasMaxLength(UserRules.MaxUsernameLength)
            .IsRequired();

        builder.Property(p => p.PasswordHash).IsRequired();

        builder
            .Property(p => p.Role)
            .HasMaxLength(UserRules.MaxRoleLength)
            .IsRequired();

        builder.HasIndex(x => x.Username).IsUnique();

        //builder.HasMany(p => p.Tasks)
        //       .WithOne()
        //       .HasForeignKey(t => t.CreatedByUserId)
        //       .OnDelete(DeleteBehavior.Cascade);
    }
}
