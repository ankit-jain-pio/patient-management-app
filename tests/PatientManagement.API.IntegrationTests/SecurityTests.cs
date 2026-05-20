using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using PatientManagement.Domain.Enums;
using Xunit;

namespace PatientManagement.API.IntegrationTests;

public class SecurityTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SecurityTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UnauthenticatedRequest_ShouldReturn401()
    {
        var response = await _client.GetAsync("/api/v1/patients");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task InvalidToken_ShouldReturn401()
    {
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "invalid_token");
        var response = await _client.GetAsync("/api/v1/patients");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturn401()
    {
        var loginRequest = new { username = "invalid", password = "wrong" };
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        var loginRequest = new { username = "admin", password = "Admin@123" };
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        result.GetProperty("token").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SqlInjection_InPatientSearch_ShouldBeSanitized()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var sqlInjectionPatterns = new[]
        {
            "' OR '1'='1",
            "'; DROP TABLE Patients--",
            "admin'--"
        };

        foreach (var pattern in sqlInjectionPatterns)
        {
            var encodedPattern = Uri.EscapeDataString(pattern);
            var response = await _client.GetAsync($"/api/v1/patients/search?searchTerm={encodedPattern}");

            response.StatusCode.Should().Be(HttpStatusCode.OK,
                $"SQL injection pattern '{pattern}' caused unexpected status code");
        }
    }

    [Fact]
    public async Task XssAttack_InPatientName_ShouldBeStored()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var xssPayload = "<script>alert('XSS')</script>";
        var createRequest = new
        {
            firstName = xssPayload,
            lastName = "Test",
            dateOfBirth = "1990-01-01",
            gender = (int)Gender.Male,
            phoneNumber = "1234567890"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/patients", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createResult = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var patientId = createResult.GetProperty("data").GetProperty("id").GetString();

        var getResponse = await _client.GetAsync($"/api/v1/patients/{patientId}");
        var getResult = await getResponse.Content.ReadFromJsonAsync<JsonElement>();

        var firstName = getResult.GetProperty("data").GetProperty("firstName").GetString();
        firstName.Should().Be(xssPayload, "Data should be stored as provided");
    }

    [Fact]
    public async Task ExcessiveDataInRequest_ShouldBeRejected()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var largeString = new string('A', 10000);
        var createRequest = new
        {
            firstName = largeString,
            lastName = largeString,
            dateOfBirth = "1990-01-01",
            gender = (int)Gender.Male,
            phoneNumber = "1234567890"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/patients", createRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var loginRequest = new { username = "admin", password = "Admin@123" };
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        return result.GetProperty("token").GetString()!;
    }
}
