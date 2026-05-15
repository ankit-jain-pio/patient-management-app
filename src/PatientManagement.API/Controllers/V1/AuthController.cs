using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PatientManagement.Application.Common.Interfaces;
using PatientManagement.Application.DTOs;
using PatientManagement.Infrastructure.Identity;

namespace PatientManagement.API.Controllers.V1;

[AllowAnonymous]
public class AuthController : BaseApiController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly ILogger<AuthController> _logger;
    
    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenGenerator tokenGenerator,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenGenerator = tokenGenerator;
        _logger = logger;
    }
    
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for user: {Username}", request.Username);
        
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            _logger.LogWarning("Login failed: User {Username} not found", request.Username);
            return Unauthorized(new { message = "Invalid username or password" });
        }
        
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Login failed: Invalid password for user {Username}", request.Username);
            return Unauthorized(new { message = "Invalid username or password" });
        }
        
        var token = _tokenGenerator.GenerateToken(user.Id, user.UserName!);
        var expiryHours = 8; // From configuration
        
        _logger.LogInformation("User {Username} logged in successfully", request.Username);
        
        return Ok(new LoginResponse(
            token,
            user.UserName!,
            DateTime.UtcNow.AddHours(expiryHours)
        ));
    }
    
    [HttpPost("validate")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult ValidateToken()
    {
        return Ok(new { message = "Token is valid", username = User.Identity?.Name });
    }
}
