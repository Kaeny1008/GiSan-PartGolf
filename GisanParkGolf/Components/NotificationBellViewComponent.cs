using Microsoft.AspNetCore.Mvc;
using GisanParkGolf.Data;
using System.Linq;

public class NotificationBellViewComponent : ViewComponent
{
    private readonly MyDbContext _dbContext;
    public NotificationBellViewComponent(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IViewComponentResult Invoke()
    {
        var userId = UserClaimsPrincipal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var notifications = _dbContext.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToList();

        return View(notifications);
    }
}