using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;


namespace TaskManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly TaskManagerContext _context;
        private readonly ILogger<TaskController> _logger;

        public TaskController(TaskManagerContext context, ILogger<TaskController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDTO>>> GetTasks()
        {
            var tasks = await _context.Tasks
                .Select(t => new TaskDTO
                {
                    TaskId = t.TaskId,
                    Name = t.Name,
                    Description = t.Description,
                    Status = t.Status,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    MemberId = t.MemberId
                })
                .ToListAsync();

            return tasks;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDTO>> GetTask(int id)
        {
            var task = await _context.Tasks
                .Where(t => t.TaskId == id)
                .Select(t => new TaskDTO
                {
                    TaskId = t.TaskId,
                    Name = t.Name,
                    Description = t.Description,
                    Status = t.Status,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    MemberId = t.MemberId
                })
                .FirstOrDefaultAsync();

            if (task == null)
            {
                _logger.LogWarning("Task with ID {TaskId} not found", id);
                return NotFound();
            }

            return task;
        }

        [HttpPost]
        public async Task<ActionResult<TaskDTO>> PostTask(TaskDTO taskDTO)
        {
            var task = new TaskManagerAPI.Models.Task
            {
                Name = taskDTO.Name,
                Description = taskDTO.Description,
                Status = taskDTO.Status,
                StartDate = taskDTO.StartDate,
                EndDate = taskDTO.EndDate,
                MemberId = taskDTO.MemberId
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            taskDTO.TaskId = task.TaskId;

            _logger.LogInformation("Created new task with ID {TaskId}", task.TaskId);
            return CreatedAtAction(nameof(GetTask), new { id = task.TaskId }, taskDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTask(int id, TaskDTO taskDTO)
        {
            if (id != taskDTO.TaskId)
            {
                _logger.LogWarning("Task ID {TaskId} does not match route ID {RouteId}", taskDTO.TaskId, id);
                return BadRequest();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                _logger.LogWarning("Task with ID {TaskId} not found", id);
                return NotFound();
            }

            task.Name = taskDTO.Name;
            task.Description = taskDTO.Description;
            task.Status = taskDTO.Status;
            task.StartDate = taskDTO.StartDate;
            task.EndDate = taskDTO.EndDate;
            task.MemberId = taskDTO.MemberId;

            _context.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    _logger.LogWarning("Task with ID {TaskId} not found during update", id);
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            _logger.LogInformation("Updated task with ID {TaskId}", task.TaskId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                _logger.LogWarning("Task with ID {TaskId} not found during deletion", id);
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted task with ID {TaskId}", id);
            return NoContent();
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskId == id);
        }
    }
}
