using Microsoft.AspNetCore.Identity;

namespace PatientManagement.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}
