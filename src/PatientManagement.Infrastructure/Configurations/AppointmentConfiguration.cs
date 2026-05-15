using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientManagement.Domain.Entities;

namespace PatientManagement.Infrastructure.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.Property(a => a.Reason)
            .HasMaxLength(500);
        
        builder.Property(a => a.Notes)
            .HasMaxLength(2000);
        
        // Query filter for soft delete
        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
