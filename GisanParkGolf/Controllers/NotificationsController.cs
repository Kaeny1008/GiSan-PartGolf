using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GisanParkGolf.Data;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
public class NotificationsController : Controller
{
    private readonly MyDbContext _dbContext;
    public NotificationsController(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("Unread")]
    public async Task<IActionResult> GetUnread()
    {
        var userId = (User as ClaimsPrincipal)?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var notifications = await _dbContext.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt) // 최신순
            .Select(n => new { userId = n.UserId, message = n.Message, createdAt = n.CreatedAt })
            .ToListAsync();

        return Ok(notifications);
    }

    [HttpPost("MarkRead")]
    public async Task<IActionResult> MarkRead(int id)
    {
        var notification = await _dbContext.Notifications
            .FirstOrDefaultAsync(n => n.NotificationId == id && !n.IsRead);

        if (notification == null)
            return NotFound();

        notification.IsRead = true;
        await _dbContext.SaveChangesAsync();

        return Ok();
    }
}