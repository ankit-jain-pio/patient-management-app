using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Infrastructure.Data;

namespace PatientManagement.API.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(ApplicationDbContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Health check endpoint for monitoring application status
    /// </summary>
    /// <returns>Health check response with system status</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Get()
    {
        var startTime = DateTime.UtcNow;
        var healthStatus = new HealthCheckResponse
        {
            Timestamp = startTime
        };

        try
        {
            // Check database connectivity
            healthStatus.Checks["database"] = await CheckDatabaseHealthAsync();

            // Check disk space
            healthStatus.Checks["diskSpace"] = CheckDiskSpace();

            // Determine overall status
            healthStatus.Status = healthStatus.Checks.Values.All(v => v == "Healthy") 
                ? "Healthy" 
                : "Unhealthy";

            healthStatus.Duration = DateTime.UtcNow - startTime;

            var statusCode = healthStatus.Status == "Healthy" 
                ? StatusCodes.Status200OK 
                : StatusCodes.Status503ServiceUnavailable;

            _logger.LogInformation("Health check completed: {Status}", healthStatus.Status);

            return StatusCode(statusCode, healthStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            
            healthStatus.Status = "Unhealthy";
            healthStatus.Error = ex.Message;
            healthStatus.Duration = DateTime.UtcNow - startTime;

            return StatusCode(StatusCodes.Status503ServiceUnavailable, healthStatus);
        }
    }

    private async Task<string> CheckDatabaseHealthAsync()
    {
        try
        {
            // Execute simple query to verify database connection
            await _context.Database.ExecuteSqlRawAsync("SELECT 1");
            return "Healthy";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return "Unhealthy";
        }
    }

    private string CheckDiskSpace()
    {
        try
        {
            var driveName = Path.GetPathRoot(AppContext.BaseDirectory);
            if (string.IsNullOrEmpty(driveName))
                return "Unknown";

            var drive = DriveInfo.GetDrives()
                .FirstOrDefault(d => d.Name.Equals(driveName, StringComparison.OrdinalIgnoreCase));

            if (drive == null)
                return "Unknown";

            // Alert if less than 1GB free space
            const long minimumFreeSpaceBytes = 1024L * 1024 * 1024; // 1 GB

            if (drive.AvailableFreeSpace < minimumFreeSpaceBytes)
            {
                _logger.LogWarning("Low disk space: {FreeSpace} MB available", 
                    drive.AvailableFreeSpace / (1024 * 1024));
                return "Unhealthy";
            }

            return "Healthy";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Disk space check failed");
            return "Unknown";
        }
    }

    public class HealthCheckResponse
    {
        public string Status { get; set; } = "Healthy";
        public Dictionary<string, string> Checks { get; set; } = new();
        public TimeSpan Duration { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Error { get; set; }
    }
}
