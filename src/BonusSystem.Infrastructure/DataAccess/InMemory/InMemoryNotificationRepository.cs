using BonusSystem.Core.Repositories;
using BonusSystem.Shared.Models;

namespace BonusSystem.Infrastructure.DataAccess.InMemory;

/// <summary>
/// In-memory implementation of the notification repository
/// </summary>
public class InMemoryNotificationRepository : INotificationRepository
{
    private readonly Dictionary<Guid, List<(string Message, NotificationType Type, DateTime Timestamp, bool IsRead)>> _notifications = new();

    public InMemoryNotificationRepository()
    {
        // Add sample notifications for existing users
        var buyer1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var seller1Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var adminId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        
        _notifications[buyer1Id] = new List<(string, NotificationType, DateTime, bool)>
        {
            ("Welcome to BonusSystem! Earn bonuses with every purchase.", NotificationType.System, DateTime.UtcNow.AddDays(-30), true),
            ("You've earned 100 bonus points from your purchase at Alpha Downtown.", NotificationType.Transaction, DateTime.UtcNow.AddDays(-10), true),
            ("You've spent 50 bonus points at Alpha Downtown.", NotificationType.Transaction, DateTime.UtcNow.AddDays(-5), false)
        };
        
        _notifications[seller1Id] = new List<(string, NotificationType, DateTime, bool)>
        {
            ("Welcome to BonusSystem! You're now a seller at Alpha Downtown.", NotificationType.System, DateTime.UtcNow.AddDays(-30), true),
            ("Transaction processed: 100 bonus points awarded to a customer.", NotificationType.Transaction, DateTime.UtcNow.AddDays(-10), true)
        };
        
        _notifications[adminId] = new List<(string, NotificationType, DateTime, bool)>
        {
            ("Welcome to BonusSystem Admin Portal!", NotificationType.System, DateTime.UtcNow.AddDays(-30), true),
            ("New store pending approval: Gamma Express", NotificationType.AdminMessage, DateTime.UtcNow.AddDays(-5), false)
        };
    }

    public Task<bool> SendNotificationAsync(Guid userId, string message, NotificationType type)
    {
        if (!_notifications.ContainsKey(userId))
        {
            _notifications[userId] = new List<(string, NotificationType, DateTime, bool)>();
        }
        
        _notifications[userId].Add((message, type, DateTime.UtcNow, false));
        return Task.FromResult(true);
    }

    public Task<IEnumerable<string>> GetUserNotificationsAsync(Guid userId)
    {
        if (!_notifications.ContainsKey(userId))
        {
            return Task.FromResult(Enumerable.Empty<string>());
        }
        
        var messages = _notifications[userId]
            .OrderByDescending(n => n.Timestamp)
            .Select(n => n.Message);
        
        return Task.FromResult(messages);
    }

    public Task<bool> MarkNotificationAsReadAsync(Guid notificationId)
    {
        // In a real implementation, we would store notification IDs
        // For the prototype, just return true
        return Task.FromResult(true);
    }

    public Task<bool> SendBulkNotificationsAsync(IEnumerable<Guid> userIds, string message, NotificationType type)
    {
        foreach (var userId in userIds)
        {
            SendNotificationAsync(userId, message, type).Wait();
        }
        
        return Task.FromResult(true);
    }

    public Task<bool> SendNotificationToRoleAsync(UserRole role, string message, NotificationType type)
    {
        // In a real implementation, we would query users by role
        // For the prototype, just return true
        return Task.FromResult(true);
    }
}