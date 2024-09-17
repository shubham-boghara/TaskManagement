using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public class TaskPriority
    {
        [Key]
        public int PriorityId { get; set; }

        public string? Name { get; set; }
    }
}
