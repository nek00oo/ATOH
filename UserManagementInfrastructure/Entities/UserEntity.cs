using UserManagementCore.Types;

namespace Infrastructure.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    
    public required string Login { get; set; }
    
    public required string Password { get; set; }
    
    public required string Name { get; set; }
    
    public required Gender Gender { get; set; } = Gender.Unknown; 
    
    public DateTime? Birthday { get; set; }
    
    public bool Admin { get; set; }
    
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    
    public required string CreatedBy { get; set; }
    
    public DateTime ModifiedOn { get; set; }
    
    public required string ModifiedBy { get; set; }
    
    public DateTime? RevokedOn { get; set; }
    
    public string? RevokedBy { get; set; }
}