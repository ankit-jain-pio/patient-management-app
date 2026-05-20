using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using PatientManagement.Domain.Enums;
using Xunit;

namespace PatientManagement.API.IntegrationTests;

public class PerformanceTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private const int MaxResponseTimeMs = 2000; // 2 seconds per requirements

    public PerformanceTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        var token = AuthenticateAsync().Result;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        SeedTestDataAsync().Wait();
    }

    private async Task<string> AuthenticateAsync()
    {
        var loginRequest = new { username = "admin", password = "Admin@123" };
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        return result.GetProperty("token").GetString()!;
    }

    private async Task SeedTestDataAsync()
    {
        for (int i = 0; i < 20; i++)
        {
            var request = new
            {
                firstName = $"Patient{i}",
                lastName = $"Test{i}",
                dateOfBirth = new DateTime(1980 + (i % 40), 1, 1).ToString("o"),
                gender = (int)(i % 2 == 0 ? Gender.Male : Gender.Female),
                phoneNumber = $"98765{i:D5}"
            };
            await _client.PostAsJsonAsync("/api/v1/patients", request);
        }
    }

    [Fact]
    public async Task PatientSearch_ShouldCompleteWithin2Seconds()
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await _client.GetAsync("/api/v1/patients/search?searchTerm=Patient");
        stopwatch.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxResponseTimeMs,
            $"Patient search took {stopwatch.ElapsedMilliseconds}ms, exceeding {MaxResponseTimeMs}ms limit");
    }

    [Fact]
    public async Task GetAllPatients_ShouldCompleteWithin2Seconds()
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await _client.GetAsync("/api/v1/patients");
        stopwatch.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxResponseTimeMs);
    }

    [Fact]
    public async Task CreatePatient_ShouldCompleteWithin2Seconds()
    {
        var request = new
        {
            firstName = "Performance",
            lastName = "Test",
            dateOfBirth = "1985-05-15",
            gender = (int)Gender.Female,
            phoneNumber = "5551234567"
        };

        var stopwatch = Stopwatch.StartNew();
        var response = await _client.PostAsJsonAsync("/api/v1/patients", request);
        stopwatch.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxResponseTimeMs);
    }

    [Fact]
    public async Task GetRecentPatients_ShouldCompleteWithin2Seconds()
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await _client.GetAsync("/api/v1/patients/recent");
        stopwatch.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxResponseTimeMs);
    }
}
