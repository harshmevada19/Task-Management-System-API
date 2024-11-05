using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Task_Management_System_API.EntityModels;

// Alias to resolve naming conflict
using TaskStatusEnum = Task_Management_System_API.EntityModels.TaskStatus;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _context;

    public TasksController(UserManager<User> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpPost]
    [Authorize] // Allow both User and Admin
    public async Task<IActionResult> CreateTask([FromBody] TaskItem model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null || user.IsDeactivated)
            return Unauthorized("User does not exist or is deactivated.");

        model.AssignedUserId = userId;
        model.Status = TaskStatusEnum.Pending; // Use alias here
        _context.Tasks.Add(model);
        await _context.SaveChangesAsync();

        return Ok("Task created successfully.");
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskItem model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var task = await _context.Tasks.FindAsync(id);

        if (task == null || (task.AssignedUserId != userId && !User.IsInRole("Admin")))
            return Unauthorized("Not authorized to edit this task.");

        task.Title = model.Title;
        task.Description = model.Description;
        task.DueDate = model.DueDate;
        task.Status = model.Status;
        await _context.SaveChangesAsync();

        return Ok("Task updated successfully.");
    }

    [HttpDelete("delete/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var task = await _context.Tasks.FindAsync(id);

        if (task == null || (task.AssignedUserId != userId && !User.IsInRole("Admin")))
            return Unauthorized("Not authorized to delete this task.");

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return Ok("Task deleted successfully.");
    }

    [HttpGet("GetAll")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllTasks()
    {
        var tasks = await _context.Tasks.Include(t => t.AssignedUser).ToListAsync();
        return Ok(tasks);
    }
}
