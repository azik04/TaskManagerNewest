using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

namespace TaskManager.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Themes>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasMany(th => th.Tasks)
                      .WithOne(t => t.Theme)
                      .HasForeignKey(t => t.ThemeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Tasks>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasMany(t => t.Files)
                      .WithOne(f => f.Task)
                      .HasForeignKey(f => f.TaskId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(t => t.UserTasks)
                      .WithOne(ut => ut.Task)
                      .HasForeignKey(ut => ut.TaskId);
            });

            modelBuilder.Entity<Files>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(f => f.Task)
                      .WithMany(t => t.Files)
                      .HasForeignKey(f => f.TaskId);
            });

            modelBuilder.Entity<UserTask>()
                .HasKey(ut => new { ut.TaskId, ut.UserId });

            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.Task)
                .WithMany(t => t.UserTasks) // Correctly link UserTask to Tasks
                .HasForeignKey(ut => ut.TaskId);

            modelBuilder.Entity<UserTask>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTasks)
                .HasForeignKey(ut => ut.UserId);
        }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Files> Files { get; set; }
        public DbSet<Themes> Themes { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserTask> UserTasks { get; set; }
    }
}
