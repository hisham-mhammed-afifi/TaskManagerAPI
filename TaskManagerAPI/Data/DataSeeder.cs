using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManagerAPI.Models;
using MyTask = TaskManagerAPI.Models.Task;

namespace TaskManagerAPI.Data
{
    public class DataSeeder
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using var context = new TaskManagerContext(
                serviceProvider.GetRequiredService<DbContextOptions<TaskManagerContext>>());

            // Look for any tasks already in the database.
            if (context.Tasks.Any() || context.TeamMembers.Any())
            {
                return;   // DB has been seeded
            }

            var teamMembers = new TeamMember[]
            {
                new TeamMember { Name = "John Doe", Email = "john.doe@example.com" },
                new TeamMember { Name = "Jane Smith", Email = "jane.smith@example.com" }
            };

            context.TeamMembers.AddRange(teamMembers);
            context.SaveChanges();

            var tasks = new MyTask[]
            {
                new MyTask { Name = "Task 1", Description = "Description for Task 1", Status = "Pending", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(7), MemberId = teamMembers[0].MemberId },
                new MyTask { Name = "Task 2", Description = "Description for Task 2", Status = "In Progress", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(14), MemberId = teamMembers[1].MemberId }
            };

            context.Tasks.AddRange(tasks);
            context.SaveChanges();
        }
    }
}
