using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_Management_System_API.EntityModels;
using System.Threading.Tasks;
using TaskStatusEnum = Task_Management_System_API.EntityModels.TaskStatus;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UserManagementController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _context;

    public UserManagementController(UserManager<User> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        return Ok(users);
    }

    [HttpPost("{userId}/assign-task")]
    public async Task<IActionResult> AssignTask(string userId, [FromBody] TaskItem model)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || user.IsDeactivated)
            return BadRequest("User does not exist or is deactivated.");

        model.AssignedUserId = userId;
        model.Status = TaskStatusEnum.Pending; 
        _context.Tasks.Add(model);
        await _context.SaveChangesAsync();

        return Ok("Task assigned successfully.");
    }

    [HttpPut("{userId}/deactivate")]
    public async Task<IActionResult> DeactivateUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound("User not found.");

        user.IsDeactivated = true;
        await _userManager.UpdateAsync(user);

        return Ok("User deactivated successfully.");
    }

    [HttpDelete("delete/{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound("User not found.");

        await _userManager.DeleteAsync(user);
        return Ok("User deleted successfully.");
    }
}
