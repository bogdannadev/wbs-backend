using BonusSystem.Core.Repositories;
using BonusSystem.Shared.Models;

namespace BonusSystem.Infrastructure.DataAccess.Postgres.Repositories;

public class PostgresNotificationRepository : INotificationRepository
{
    public Task<bool> SendNotificationAsync(Guid userId, string message, NotificationType type)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> GetUserNotificationsAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> MarkNotificationAsReadAsync(Guid notificationId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SendBulkNotificationsAsync(IEnumerable<Guid> userIds, string message, NotificationType type)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SendNotificationToRoleAsync(UserRole role, string message, NotificationType type)
    {
        throw new NotImplementedException();
    }
}