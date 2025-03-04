namespace BonusSystem.Shared.Models;

public enum UserRole
{
    Buyer,
    Seller,
    StoreAdmin,
    SystemAdmin,
    CompanyObserver,
    SystemObserver
}

public enum CompanyStatus
{
    Active,
    Suspended,
    Pending
}

public enum StoreStatus
{
    Active,
    Inactive,
    PendingApproval
}

public enum TransactionType
{
    Earn,
    Spend,
    Expire,
    AdminAdjustment
}

public enum TransactionStatus
{
    Pending,
    Completed,
    Reversed,
    Failed
}

public enum NotificationType
{
    Transaction,
    System,
    Expiration,
    AdminMessage
}