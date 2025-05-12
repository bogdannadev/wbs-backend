using BonusSystem.Shared.Models; 
using BonusSystem.Shared.Dtos; 

namespace BonusSystem.Infrastructure.DataAccess.Entities; 

public class FiatTransaction 
{ 
    public Guid Id {get; set; }  
    public Guid BuyerId {get; set; }
    public PaymentRequest PaymentBody {get; set; } 
    public string Description { get; set; } = string.Empty; 
    public DateTime TransactionDate { get; set; }  
    public TransactionStatus Status {get; set; } 

    // Navigation properties

    public UserEntity? User { get; set; }
    public CompanyEntity? Company { get; set; }
    public StoreEntity? Store { get; set; }
}