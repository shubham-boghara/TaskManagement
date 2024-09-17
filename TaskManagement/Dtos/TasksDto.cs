using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.Dtos
{
    public class TasksDto
    {
    }

    public class CreateTasksDto
    {
        //public int TaskId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string? Status { get; set; }

        public DateTime? DueDate { get; set; }

        [Required]
        public int? PriorityId { get; set; }
    }

    public class UpdateTasksDto
    {
        [Required]
        public int TaskId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string? Status { get; set; }

        public DateTime? DueDate { get; set; }

        [Required]
        public int? PriorityId { get; set; }
    }

    public class UpdateTasksStatusDto 
    {
        [Required]
        public int TaskId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string? Status { get; set; }
    }
}
