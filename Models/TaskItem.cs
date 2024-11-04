using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace TaskManagementBackend.Controllers
{
    public class TaskItem
    {
        [Key]
        public int TaskId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Description { get; set; }

        public bool IsComplete { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DueDate { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; }

        public string TaggedUsersJson { get; set; }

        [NotMapped]
        public List<string> TaggedUsers
        {
            get => TaggedUsersJson == null ? new List<string>() : JsonSerializer.Deserialize<List<string>>(TaggedUsersJson);
            set => TaggedUsersJson = JsonSerializer.Serialize(value);
        }
    }

}

