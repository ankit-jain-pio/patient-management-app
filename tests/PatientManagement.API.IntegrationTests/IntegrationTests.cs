using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using PatientManagement.Domain.Enums;
using Xunit;

namespace PatientManagement.API.IntegrationTests;

public class IntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public IntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        // Authenticate
        var token = AuthenticateAsync().Result;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<string> AuthenticateAsync()
    {
        var loginRequest = new { username = "admin", password = "Admin@123" };
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        return result.GetProperty("token").GetString()!;
    }

    [Fact]
    public async Task EndToEndWorkflow_CreatePatientAppointmentConsultation_ShouldSucceed()
    {
        // Step 1: Create patient
        var patientRequest = new
        {
            firstName = "Integration",
            lastName = "Test",
            dateOfBirth = "1985-05-15",
            gender = (int)Gender.Male,
            phoneNumber = "9876543210"
        };

        var patientResponse = await _client.PostAsJsonAsync("/api/v1/patients", patientRequest);
        patientResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var patientResult = await patientResponse.Content.ReadFromJsonAsync<JsonElement>();
        var patientId = Guid.Parse(patientResult.GetProperty("data").GetProperty("id").GetString()!);

        // Step 2: Create appointment
        var appointmentRequest = new
        {
            patientId,
            scheduledDateTime = DateTime.UtcNow.AddDays(1).ToString("o"),
            reason = "Regular checkup"
        };

        var appointmentResponse = await _client.PostAsJsonAsync("/api/v1/appointments", appointmentRequest);
        appointmentResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Step 3: Create consultation
        var consultationRequest = new
        {
            patientId,
            chiefComplaint = "Fever and headache",
            symptoms = "High temperature for 3 days",
            vitals = new
            {
                temperature = 38.5m,
                bloodPressure = "130/85",
                pulseRate = 88,
                respiratoryRate = "18",
                oxygenSaturation = 97m
            }
        };

        var consultationResponse = await _client.PostAsJsonAsync("/api/v1/consultations", consultationRequest);
        consultationResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        // Step 4: Verify consultation was created
        var consultationResult = await consultationResponse.Content.ReadFromJsonAsync<JsonElement>();
        var consultationId = Guid.Parse(consultationResult.GetProperty("data").GetProperty("id").GetString()!);

        var getConsultationResponse = await _client.GetAsync($"/api/v1/consultations/{consultationId}");
        getConsultationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreatePatient_WithInvalidData_ShouldReturnBadRequest()
    {
        var invalidRequest = new
        {
            firstName = "", // Empty - invalid
            lastName = "Test",
            dateOfBirth = DateTime.UtcNow.AddDays(1).ToString("o"), // Future date - invalid
            gender = (int)Gender.Male,
            phoneNumber = "123" // Too short - invalid
        };

        var response = await _client.PostAsJsonAsync("/api/v1/patients", invalidRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPatient_WithNonExistentId_ShouldReturnNotFound()
    {
        var response = await _client.GetAsync($"/api/v1/patients/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
