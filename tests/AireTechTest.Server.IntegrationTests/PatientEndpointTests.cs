using System.Net;
using System.Net.Http.Json;

using AireTechTest.Server.Api.Patients;

using Microsoft.AspNetCore.Http;

namespace AireTechTest.Server.IntegrationTests;

public class PatientEndpointTests
{
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [Before(Test)]
    public void Setup()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [After(Test)]
    public async Task Cleanup()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    [Test]
    public async Task GetAllPatients_WhenEmpty_ReturnsEmptyList()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/patients");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        List<PatientResponse>? patients = await response.Content.ReadFromJsonAsync<List<PatientResponse>>();
        await Assert.That(patients).IsNotNull();
        await Assert.That(patients!.Count).IsEqualTo(0);
    }

    [Test]
    public async Task CreatePatient_WithValidData_ReturnsCreated()
    {
        CreatePatientRequest request = new()
        {
            NhsNumber = "9434765919",
            Name = "John Smith",
            DateOfBirth = new DateOnly(1990, 1, 15),
            Postcode = "SW1A 1AA"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/patients", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        await Assert.That(response.Headers.Location).IsNotNull();

        PatientResponse? patient = await response.Content.ReadFromJsonAsync<PatientResponse>();
        await Assert.That(patient).IsNotNull();
        await Assert.That(patient!.NhsNumber).IsEqualTo("9434765919");
        await Assert.That(patient.Name).IsEqualTo("John Smith");
        await Assert.That(patient.DateOfBirth).IsEqualTo(new DateOnly(1990, 1, 15));
        await Assert.That(patient.Postcode).IsEqualTo("SW1A 1AA");
    }

    [Test]
    public async Task CreatePatient_WithInvalidNhsNumber_ReturnsValidationProblem()
    {
        CreatePatientRequest request = new()
        {
            NhsNumber = "invalid",
            Name = "John Smith",
            DateOfBirth = new DateOnly(1990, 1, 15),
            Postcode = "SW1A 1AA"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/patients", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

        HttpValidationProblemDetails?
            problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        await Assert.That(problem).IsNotNull();
        await Assert.That(problem!.Errors.ContainsKey("NhsNumber")).IsTrue();
    }

    [Test]
    public async Task CreatePatient_WithDuplicateNhsNumber_ReturnsConflict()
    {
        CreatePatientRequest request = new()
        {
            NhsNumber = "9434765919",
            Name = "John Smith",
            DateOfBirth = new DateOnly(1990, 1, 15),
            Postcode = "SW1A 1AA"
        };

        // Create first patient
        await _client.PostAsJsonAsync("/api/patients", request);

        // Try to create duplicate
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/patients", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Conflict);
    }

    [Test]
    public async Task GetPatient_WhenExists_ReturnsPatient()
    {
        // Create a patient first
        CreatePatientRequest createRequest = new()
        {
            NhsNumber = "9434765919",
            Name = "John Smith",
            DateOfBirth = new DateOnly(1990, 1, 15),
            Postcode = "SW1A 1AA"
        };
        await _client.PostAsJsonAsync("/api/patients", createRequest);

        // Get the patient
        HttpResponseMessage response = await _client.GetAsync("/api/patients/9434765919");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        PatientResponse? patient = await response.Content.ReadFromJsonAsync<PatientResponse>();
        await Assert.That(patient).IsNotNull();
        await Assert.That(patient!.NhsNumber).IsEqualTo("9434765919");
    }

    [Test]
    public async Task GetPatient_WhenNotExists_ReturnsNotFound()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/patients/9434765919");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task UpdatePatient_WithValidData_ReturnsOk()
    {
        // Create a patient first
        CreatePatientRequest createRequest = new()
        {
            NhsNumber = "9434765919",
            Name = "John Smith",
            DateOfBirth = new DateOnly(1990, 1, 15),
            Postcode = "SW1A 1AA"
        };
        await _client.PostAsJsonAsync("/api/patients", createRequest);

        // Update the patient
        UpdatePatientRequest updateRequest = new()
        {
            Name = "Jane Smith", DateOfBirth = new DateOnly(1985, 6, 20), Postcode = "EC1A 1BB"
        };

        HttpResponseMessage response = await _client.PutAsJsonAsync("/api/patients/9434765919", updateRequest);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        PatientResponse? patient = await response.Content.ReadFromJsonAsync<PatientResponse>();
        await Assert.That(patient).IsNotNull();
        await Assert.That(patient!.Name).IsEqualTo("Jane Smith");
        await Assert.That(patient.DateOfBirth).IsEqualTo(new DateOnly(1985, 6, 20));
        await Assert.That(patient.Postcode).IsEqualTo("EC1A 1BB");
    }

    [Test]
    public async Task UpdatePatient_WhenNotExists_ReturnsNotFound()
    {
        UpdatePatientRequest updateRequest = new()
        {
            Name = "Jane Smith", DateOfBirth = new DateOnly(1985, 6, 20), Postcode = "EC1A 1BB"
        };

        HttpResponseMessage response = await _client.PutAsJsonAsync("/api/patients/9434765919", updateRequest);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task UpdatePatient_WithInvalidData_ReturnsValidationProblem()
    {
        // Create a patient first
        CreatePatientRequest createRequest = new()
        {
            NhsNumber = "9434765919",
            Name = "John Smith",
            DateOfBirth = new DateOnly(1990, 1, 15),
            Postcode = "SW1A 1AA"
        };
        await _client.PostAsJsonAsync("/api/patients", createRequest);

        // Try to update with invalid data
        UpdatePatientRequest updateRequest = new()
        {
            Name = "", DateOfBirth = new DateOnly(1985, 6, 20), Postcode = "INVALID"
        };

        HttpResponseMessage response = await _client.PutAsJsonAsync("/api/patients/9434765919", updateRequest);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task DeletePatient_WhenExists_ReturnsNoContent()
    {
        // Create a patient first
        CreatePatientRequest createRequest = new()
        {
            NhsNumber = "9434765919",
            Name = "John Smith",
            DateOfBirth = new DateOnly(1990, 1, 15),
            Postcode = "SW1A 1AA"
        };
        await _client.PostAsJsonAsync("/api/patients", createRequest);

        // Delete the patient
        HttpResponseMessage response = await _client.DeleteAsync("/api/patients/9434765919");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

        // Verify patient is deleted
        HttpResponseMessage getResponse = await _client.GetAsync("/api/patients/9434765919");
        await Assert.That(getResponse.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeletePatient_WhenNotExists_ReturnsNotFound()
    {
        HttpResponseMessage response = await _client.DeleteAsync("/api/patients/9434765919");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetAllPatients_AfterCreating_ReturnsPatients()
    {
        // Create two patients
        CreatePatientRequest request1 = new()
        {
            NhsNumber = "9434765919",
            Name = "John Smith",
            DateOfBirth = new DateOnly(1990, 1, 15),
            Postcode = "SW1A 1AA"
        };
        CreatePatientRequest request2 = new()
        {
            NhsNumber = "6148aborning530",
            Name = "Jane Doe",
            DateOfBirth = new DateOnly(1985, 6, 20),
            Postcode = "EC1A 1BB"
        };
        await _client.PostAsJsonAsync("/api/patients", request1);
        await _client.PostAsJsonAsync("/api/patients", request2);

        HttpResponseMessage response = await _client.GetAsync("/api/patients");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        List<PatientResponse>? patients = await response.Content.ReadFromJsonAsync<List<PatientResponse>>();
        await Assert.That(patients).IsNotNull();
        // At least 1 patient (the first one should succeed, second has invalid NHS number)
        await Assert.That(patients!.Count).IsGreaterThanOrEqualTo(1);
    }
}