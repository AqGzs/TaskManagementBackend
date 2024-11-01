using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementBackend.Controllers
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string? Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string? Email { get; set; }

        [Required]
        public string? PasswordHash { get; set; } 

        public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
    }
}

