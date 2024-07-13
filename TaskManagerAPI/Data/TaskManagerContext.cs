using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Data
{
    public class TaskManagerContext : DbContext
    {
        public TaskManagerContext(DbContextOptions<TaskManagerContext> options)
            : base(options)
        {
        }

        public DbSet<TaskManagerAPI.Models.Task> Tasks { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskManagerAPI.Models.Task>()
                .Property(t => t.Name)
                .IsRequired();

            modelBuilder.Entity<TaskManagerAPI.Models.Task>()
                .Property(t => t.Description)
                .IsRequired();

            modelBuilder.Entity<TeamMember>()
                .HasMany(m => m.Tasks)
                .WithOne(t => t.TeamMember)
                .HasForeignKey(t => t.MemberId);

            modelBuilder.Entity<TeamMember>()
                .Property(m => m.Email)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(256);
        }
    }
}
