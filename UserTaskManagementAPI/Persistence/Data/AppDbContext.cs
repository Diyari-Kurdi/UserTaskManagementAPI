using Microsoft.EntityFrameworkCore;
using UserTaskManagementAPI.Domain.Entities;
using UserTaskManagementAPI.Infrastructure.Configurations;

namespace UserTaskManagementAPI.Persistence.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new TaskItemConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}