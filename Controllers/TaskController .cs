using global::TaskManagementBackend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagementBackend.Controllers;

    namespace TaskManagementBackend.Controllers
    {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Task - Lấy danh sách tất cả các TaskItem của người dùng
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
        {
            // Giả sử bạn đang lấy UserId từ token đã xác thực
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            return await _context.TaskItems
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        // GET: api/Task/{id} - Lấy thông tin một TaskItem cụ thể của người dùng
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

            int tokenUserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            // Ensure the UserId in the request matches the one in the token
            if (taskItem.UserId != tokenUserId)
            {
                return Unauthorized("User ID in token does not match User ID in request body.");
            }

            taskItem.UserId = tokenUserId;  // Gán lại UserId từ token cho TaskItem để đảm bảo chính xác

            _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = taskItem.TaskId }, taskItem);
        }



        // PUT: api/Task/{id} - Cập nhật một TaskItem của người dùng
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskItem taskItem)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            if (id != taskItem.TaskId)
            {
                return BadRequest();
            }

            // Kiểm tra xem task thuộc về người dùng hiện tại không
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

            return NoContent();
        }

        // DELETE: api/Task/{id} - Xóa một TaskItem của người dùng
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

            taskItem.IsComplete = isComplete; // Cập nhật trạng thái hoàn thành
            await _context.SaveChangesAsync();

            return NoContent(); // Trả về 204 No Content khi cập nhật thành công
        }

    }

}

