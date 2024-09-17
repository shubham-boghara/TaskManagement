using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.Models
{
    public class Tasks
    {
        [Key]
        public int TaskId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }

        public string? CreatedByUserId { get; set; }

        [ForeignKey("TaskPriority")]
        public int? PriorityId { get; set; }
        public virtual TaskPriority TaskPriority { get; set; }
    }


    [Table("Vm_Task")]
    public class Vm_Task
    {
        [Key]
        public int TaskId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }

        public string? CreatedByUserId { get; set; }
        public int? PriorityId { get; set; }
        public string? Name { get; set; }
    }
}
