using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientManagement.Domain.Entities;

namespace PatientManagement.Infrastructure.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(p => p.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(p => p.Email)
            .HasMaxLength(255);
        
        builder.Property(p => p.BloodGroup)
            .HasMaxLength(10);
        
        // Query filter for soft delete
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
