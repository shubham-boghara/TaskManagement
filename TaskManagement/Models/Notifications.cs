using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.Models
{
    public class Notifications
    {
        [Key]
        public int NotificationId { get; set; }

        [ForeignKey("Tasks")]
        public int? TaskId { get; set; }

        public string? UserId { get; set; }  

        public string? Message { get; set; }

        public bool? IsRead { get; set; }

        public DateTime? CreatedDate { get; set; }

        public virtual Tasks Tasks { get; set; }

    }
}
