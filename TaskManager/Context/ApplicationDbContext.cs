using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

namespace TaskManager.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasData(
                new Users
                {
                    Email = "Admin@gmail.com",
                    UserName = "Admin",
                    Password = "Admin123",
                    Id = 1,
                    Role = Enum.Role.Admin
                });
        });

        modelBuilder.Entity<Themes>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(t => t.Users)
                .WithMany(x => x.Themes)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(th => th.Tasks)
                .WithOne(t => t.Theme)
                .HasForeignKey(t => t.ThemeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Tasks>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(t => t.ExecutiveUser)
                  .WithMany(u => u.Tasks)
                  .HasForeignKey(t => t.ExecutiveUserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(t => t.UserTasks)
                  .WithOne(ut => ut.Task)
                  .HasForeignKey(ut => ut.TaskId);

            entity.HasMany(t => t.Comments) 
                  .WithOne(c => c.Tasks)
                  .HasForeignKey(c => c.TaskId);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.HasOne(c => c.User)
                  .WithMany(u => u.Comment)
                  .HasForeignKey(c => c.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Files>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(f => f.Task)
                .WithMany(t => t.Files)
                .HasForeignKey(f => f.TaskId);
        });

        modelBuilder.Entity<UserTask>(entity =>
        {
            entity.HasKey(ut => new { ut.TaskId, ut.UserId });

            entity.HasOne(ut => ut.Task)
                .WithMany(t => t.UserTasks)
                .HasForeignKey(ut => ut.TaskId);

            entity.HasOne(ut => ut.User)
                .WithMany(u => u.UserTasks)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

    }



    public DbSet<Tasks> Tasks { get; set; }
    public DbSet<Files> Files { get; set; }
    public DbSet<Themes> Themes { get; set; }
    public DbSet<Users> Users { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<UserTask> UserTasks { get; set; }
}
