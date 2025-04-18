using BonusSystem.Shared.Models;

namespace BonusSystem.Shared.Dtos;

public record struct NotificationRequest()
{
    public string Message { get; init; } = string.Empty;
    public NotificationType Type { get; init; } = NotificationType.System;
}