using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.Models
{
    public class TaskUpdates
    {

        [Key]
        public int UpdateId { get; set; }

        [ForeignKey("Tasks")]
        public int? TaskId { get; set; }

        public string UpdatedByUserId { get; set; }

        public string Status { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? Notes { get; set; }

        public virtual Tasks Tasks { get; set; }
    }
}
