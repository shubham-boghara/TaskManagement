using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.Models
{
    public class TaskAssignments
    {
        [Key]
        public int AssignmentId { get; set; }

        [ForeignKey("Tasks")]
        public int? TaskId { get; set; }

        public string? AssignedToUserId { get; set; }

        public string? AssignedByUserId { get; set; }

        public DateTime? AssignedDate { get; set; } 

        public virtual Tasks Tasks { get; set; }
    }

    [Table("Vm_TaskAssignmentsWithTask")]
    public class Vm_TaskAssignmentsWithTask
    {
        
        public int TaskId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }

        public string? CreatedByUserId { get; set; }
        public int? PriorityId { get; set; }
        public string? Name { get; set; }

        [Key]
        public int AssignmentId { get; set; }

        public string? AssignedToUserId { get; set; }

        public string? AssignedByUserId { get; set; }

        public DateTime? AssignedDate { get; set; }
    }
}
