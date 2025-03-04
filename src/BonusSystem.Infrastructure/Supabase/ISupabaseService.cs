using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;

namespace BonusSystem.Infrastructure.Supabase;

public interface ISupabaseService
{
    // Authentication
    Task<(bool Success, string? ErrorMessage, string? Token, Guid? UserId)> SignInAsync(string email, string password);
    Task<(bool Success, string? ErrorMessage, string? Token, Guid? UserId)> SignUpAsync(string email, string password, UserRole role);
    Task<bool> SignOutAsync(string token);
    
    // User Management
    Task<UserDto?> GetUserByIdAsync(Guid userId);
    Task<UserRole> GetUserRoleAsync(Guid userId);
    Task<bool> UpdateUserAsync(UserDto user);
    
    // Company Management
    Task<CompanyDto?> GetCompanyByIdAsync(Guid companyId);
    Task<Guid> CreateCompanyAsync(CompanyRegistrationDto company);
    Task<bool> UpdateCompanyStatusAsync(Guid companyId, CompanyStatus status);
    Task<bool> UpdateCompanyBalanceAsync(Guid companyId, decimal newBalance);
    Task<IEnumerable<CompanyDto>> GetCompaniesAsync();
    
    // Store Management
    Task<StoreDto?> GetStoreByIdAsync(Guid storeId);
    Task<Guid> CreateStoreAsync(StoreRegistrationDto store);
    Task<bool> UpdateStoreStatusAsync(Guid storeId, StoreStatus status);
    Task<IEnumerable<StoreDto>> GetStoresByCompanyIdAsync(Guid companyId);
    Task<IEnumerable<StoreDto>> GetStoresByCategoryAsync(string category);
    
    // Transaction Management
    Task<TransactionDto?> GetTransactionByIdAsync(Guid transactionId);
    Task<Guid> CreateTransactionAsync(TransactionRequestDto transaction);
    Task<bool> UpdateTransactionStatusAsync(Guid transactionId, TransactionStatus status);
    Task<IEnumerable<TransactionDto>> GetTransactionsByUserIdAsync(Guid userId);
    Task<IEnumerable<TransactionDto>> GetTransactionsByCompanyIdAsync(Guid companyId);
    Task<IEnumerable<TransactionDto>> GetTransactionsByStoreIdAsync(Guid storeId);
    
    // Notifications
    Task<bool> SendNotificationAsync(Guid userId, string message, NotificationType type);
    Task<IEnumerable<string>> GetUserNotificationsAsync(Guid userId);
}