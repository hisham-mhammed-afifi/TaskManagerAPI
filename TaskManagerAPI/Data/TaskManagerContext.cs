using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using TaskManagerAPI.Models;
using MyTask = TaskManagerAPI.Models.Task;

namespace TaskManagerAPI.Data
{
    public class TaskManagerContext : DbContext
    {
        public TaskManagerContext(DbContextOptions<TaskManagerContext> options)
            : base(options)
        {
        }

        public DbSet<MyTask> Tasks { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MyTask>()
                .Property(t => t.Name)
                .IsRequired();

            modelBuilder.Entity<MyTask>()
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
