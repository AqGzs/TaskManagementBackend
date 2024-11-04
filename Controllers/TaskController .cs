using global::TaskManagementBackend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace TaskManagementBackend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public TaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
        {
            // Giả sử bạn đang lấy UserId từ token đã xác thực
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            return await _context.TaskItems
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTask(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            var taskItem = await _context.TaskItems
                .Where(t => t.UserId == userId && t.TaskId == id)
                .FirstOrDefaultAsync();

            if (taskItem == null)
            {
                return NotFound();
            }

            return taskItem;
        }
        [HttpPost]
        public async Task<ActionResult<TaskItem>> CreateTask([FromBody] TaskItem taskItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Lấy userId từ token đã xác thực
            int tokenUserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            taskItem.UserId = tokenUserId; // Chỉ đặt UserId, không bao gồm đối tượng User

            // Đảm bảo không có tham chiếu tới đối tượng User khi lưu
            taskItem.User = null;

            _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync();

            if (taskItem.TaggedUsers.Any())
            {
                await _emailService.SendTaggedNotificationEmail(taskItem.TaggedUsers, taskItem);
            }

            return CreatedAtAction(nameof(GetTask), new { id = taskItem.TaskId }, taskItem);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskItem taskItem)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            if (id != taskItem.TaskId)
            {
                return BadRequest();
            }

            var existingTask = await _context.TaskItems.FirstOrDefaultAsync(t => t.TaskId == id && t.UserId == userId);
            if (existingTask == null)
            {
                return NotFound();
            }

            existingTask.Title = taskItem.Title;
            existingTask.Description = taskItem.Description;
            existingTask.IsComplete = taskItem.IsComplete;
            existingTask.DueDate = taskItem.DueDate;

            await _context.SaveChangesAsync();

            if (taskItem.TaggedUsers.Any())
            {
                await _emailService.SendTaggedNotificationEmail(taskItem.TaggedUsers, taskItem);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            var taskItem = await _context.TaskItems.FirstOrDefaultAsync(t => t.TaskId == id && t.UserId == userId);
            if (taskItem == null)
            {
                return NotFound();
            }

            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();



            return NoContent();
        }


        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> MarkTaskAsComplete(int id, [FromBody] bool isComplete)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            var taskItem = await _context.TaskItems.FirstOrDefaultAsync(t => t.TaskId == id && t.UserId == userId);

            if (taskItem == null)
            {
                return NotFound();
            }

            taskItem.IsComplete = isComplete;
            await _context.SaveChangesAsync();

            return NoContent(); 
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasksByCompletionStatus(bool isComplete)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            var tasks = await _context.TaskItems
                .Where(t => t.UserId == userId && t.IsComplete == isComplete)
                .ToListAsync();

            return tasks;
        }

    }
}

