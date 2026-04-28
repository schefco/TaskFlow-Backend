using Microsoft.EntityFrameworkCore;
using Schefco.TaskFlow.Domain.Entities;

namespace Schefco.TaskFlow.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Project> Projects => Set<Project>();
        public DbSet<AppUser> Users { get; set; }
        public DbSet<PendingUser> PendingUsers { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<CommentEntity> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Status).HasConversion<int>();

                entity.Property(p => p.Priority).HasConversion<int>();

                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);

                entity.Property(p => p.Description).HasMaxLength(2000);
            });

            // Configures the relationship table between tasks and subtasks
            // prevents cascading deletes from wiping out entire task trees
            modelBuilder.Entity<TaskEntity>()
                .HasOne(t => t.ParentTask)
                .WithMany(t => t.Subtasks)
                .HasForeignKey(t => t.ParentTaskId)
                .OnDelete(DeleteBehavior.Restrict);


            // Configures comments table for task comments with a foreign key of Tasks
            modelBuilder.Entity<CommentEntity>(entity =>
            {
                entity.ToTable("Comments");

                entity.HasKey(c => c.Id);

                entity.Property(c => c.Content).IsRequired().HasMaxLength(500);

                entity.HasOne(c => c.Task)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            // Configures default sorting order
            modelBuilder.Entity<TaskEntity>()
                .Property(t => t.SortOrder)
                .HasDefaultValue(0);

            // Configures the relationship table between tasks and comments
            // enables cascading delete of comments with tasks
            modelBuilder.Entity<TaskEntity>()
                .HasMany(t => t.Comments)
                .WithOne(c => c.Task)
                .HasForeignKey(c => c.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
