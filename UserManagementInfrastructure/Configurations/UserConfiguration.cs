using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagementCore.Types;

namespace Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.Id);

        builder.Property(u => u.Login)
            .IsRequired();
        
        builder.Property(u => u.Password)
            .IsRequired();
        
        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(u => u.Gender)
            .IsRequired();
        
        builder.Property(u => u.Admin)
            .IsRequired();
        
        builder.HasIndex(u => u.Login)
            .IsUnique();
        
        builder.HasData(
            new UserEntity
            {
                Id = Guid.NewGuid(),
                Login = "admin",
                Password = "hashed_password",
                Gender = Gender.Man,
                Name = "Admin",
                Admin = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "system",
                ModifiedBy = "system",
                ModifiedOn = DateTime.UtcNow,
            });
    }
}