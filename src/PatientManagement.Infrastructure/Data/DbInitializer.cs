using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PatientManagement.Infrastructure.Identity;

namespace PatientManagement.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        
        // Apply migrations
        await context.Database.MigrateAsync();
        
        // Seed default user (single physician)
        if (!await userManager.Users.AnyAsync())
        {
            var defaultUser = new ApplicationUser
            {
                UserName = "doctor",
                Email = "doctor@clinic.com",
                FullName = "Dr. Clinic Admin",
                EmailConfirmed = true
            };
            
            // Default password: Doctor@123 (to be changed on first login)
            var result = await userManager.CreateAsync(defaultUser, "Doctor@123");
            
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create default user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}
