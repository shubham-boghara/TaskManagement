using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Dtos
{
    public class TaskAssignmentsDto
    {
    }

    public class CreateTaskAssignmentsDto
    {
        [Required]
        public int TaskId { get; set; }

        [Required]
        public string AssignedToUserId { get; set; }

        /*[Required]
        public int AssignedByUserId { get; set; }*/
    }
}
