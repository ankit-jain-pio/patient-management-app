using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using PatientManagement.Infrastructure.Identity;
using Xunit;
using FluentAssertions;

namespace PatientManagement.Infrastructure.Tests.Identity;

public class JwtTokenGeneratorTests
{
    private readonly IConfiguration _configuration;
    
    public JwtTokenGeneratorTests()
    {
        var configValues = new Dictionary<string, string>
        {
            {"JwtSettings:SecretKey", "TestSecretKeyForJWTTokenGeneration123456!"},
            {"JwtSettings:Issuer", "TestIssuer"},
            {"JwtSettings:Audience", "TestAudience"},
            {"JwtSettings:ExpiryHours", "8"}
        };
        
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues!)
            .Build();
    }
    
    [Fact]
    public void GenerateToken_ShouldReturnValidToken_WhenCalledWithValidCredentials()
    {
        // Arrange
        var generator = new JwtTokenGenerator(_configuration);
        var userId = Guid.NewGuid().ToString();
        var username = "testuser";
        
        // Act
        var token = generator.GenerateToken(userId, username);
        
        // Assert
        token.Should().NotBeNullOrEmpty();
        
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId);
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.UniqueName && c.Value == username);
    }
    
    [Fact]
    public void GenerateToken_ShouldSetCorrectExpiry_WhenGenerated()
    {
        // Arrange
        var generator = new JwtTokenGenerator(_configuration);
        var userId = Guid.NewGuid().ToString();
        var username = "testuser";
        
        // Act
        var token = generator.GenerateToken(userId, username);
        
        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        
        var expiryDifference = jwtToken.ValidTo - DateTime.UtcNow;
        expiryDifference.TotalHours.Should().BeApproximately(8, 0.1);
    }
}
