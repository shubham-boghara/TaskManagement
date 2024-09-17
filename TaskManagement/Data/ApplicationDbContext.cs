using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Models;

namespace TaskManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tasks> Tasks { get; set; }

        public DbSet<Notifications> Notifications { get; set; }

        public DbSet<TaskAssignments> TaskAssignments { get; set; }

        public DbSet<TaskPriority> TaskPriority { get; set; }

        public DbSet<TaskUpdates> TaskUpdates { get; set; }

        public DbSet<Vm_TaskAssignmentsWithTask> Vm_TaskAssignmentsWithTask { get; set; }

        public DbSet<Vm_Task> Vm_Tasks { get; set; }
    }
}
