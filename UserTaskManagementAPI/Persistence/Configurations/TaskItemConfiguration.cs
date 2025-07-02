using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserTaskManagementAPI.Domain.Entities;
using UserTaskManagementAPI.Domain.Rules;

namespace UserTaskManagementAPI.Infrastructure.Configurations;

internal class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(p => p.Title)
            .HasMaxLength(TaskItemRules.MaxTitleLength)
            .IsRequired();

        builder
            .Property(p => p.Description)
            .HasMaxLength(TaskItemRules.MaxDescriptionLength);

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(p => p.CreatedByUserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.CreatedByUserId);
    }
}
