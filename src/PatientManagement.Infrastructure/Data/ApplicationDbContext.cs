using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PatientManagement.Application.Common.Interfaces;
using PatientManagement.Domain.Entities;
using PatientManagement.Infrastructure.Identity;

namespace PatientManagement.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Consultation> Consultations => Set<Consultation>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Apply entity configurations
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        // Configure Value Objects as owned entities
        builder.Entity<Patient>().OwnsOne(p => p.Address);
        builder.Entity<Consultation>().OwnsOne(c => c.Vitals);
        
        // Configure relationships
        builder.Entity<Patient>()
            .HasMany(p => p.Appointments)
            .WithOne(a => a.Patient)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Patient>()
            .HasMany(p => p.Consultations)
            .WithOne(c => c.Patient)
            .HasForeignKey(c => c.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Consultation>()
            .HasMany(c => c.Prescriptions)
            .WithOne(p => p.Consultation)
            .HasForeignKey(p => p.ConsultationId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Entity<Appointment>()
            .HasOne(a => a.Consultation)
            .WithOne(c => c.Appointment)
            .HasForeignKey<Consultation>(c => c.AppointmentId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Indexes for performance
        builder.Entity<Patient>()
            .HasIndex(p => p.PhoneNumber);
        
        builder.Entity<Patient>()
            .HasIndex(p => new { p.FirstName, p.LastName });
        
        builder.Entity<Appointment>()
            .HasIndex(a => a.ScheduledDateTime);
        
        builder.Entity<Consultation>()
            .HasIndex(c => c.ConsultationDate);
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps
        var entries = ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }
}
