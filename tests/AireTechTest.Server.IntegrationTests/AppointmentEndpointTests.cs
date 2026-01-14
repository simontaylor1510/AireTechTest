using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

using AireTechTest.Server.Api.Appointments;
using AireTechTest.Server.Api.Patients;
using AireTechTest.Server.Domain;

using Microsoft.AspNetCore.Http;

namespace AireTechTest.Server.IntegrationTests;

public class AppointmentEndpointTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

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

    private async Task CreateTestPatient()
    {
        CreatePatientRequest request = new()
        {
            NhsNumber = "9434765919",
            Name = "John Smith",
            DateOfBirth = new DateOnly(1990, 1, 15),
            Postcode = "SW1A 1AA"
        };
        await _client.PostAsJsonAsync("/api/patients", request);
    }

    [Test]
    public async Task GetAllAppointments_WhenEmpty_ReturnsEmptyList()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/appointments");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        List<AppointmentResponse>? appointments = await response.Content.ReadFromJsonAsync<List<AppointmentResponse>>(JsonOptions);
        await Assert.That(appointments).IsNotNull();
        await Assert.That(appointments!.Count).IsEqualTo(0);
    }

    [Test]
    public async Task CreateAppointment_WithValidData_ReturnsCreated()
    {
        await CreateTestPatient();

        CreateAppointmentRequest request = new()
        {
            PatientNhsNumber = "9434765919",
            Status = AppointmentStatus.Active,
            Time = DateTimeOffset.Now.AddDays(1),
            DurationMinutes = 30,
            Clinician = "Dr. Smith",
            Department = Department.Oncology,
            Location = "SW1A 1AA"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/appointments", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        await Assert.That(response.Headers.Location).IsNotNull();

        AppointmentResponse? appointment = await response.Content.ReadFromJsonAsync<AppointmentResponse>(JsonOptions);
        await Assert.That(appointment).IsNotNull();
        await Assert.That(appointment!.PatientNhsNumber).IsEqualTo("9434765919");
        await Assert.That(appointment.Clinician).IsEqualTo("Dr. Smith");
        await Assert.That(appointment.DurationMinutes).IsEqualTo(30);
    }

    [Test]
    public async Task CreateAppointment_WithNonExistentPatient_ReturnsNotFound()
    {
        CreateAppointmentRequest request = new()
        {
            PatientNhsNumber = "9434765919",
            Status = AppointmentStatus.Active,
            Time = DateTimeOffset.Now.AddDays(1),
            DurationMinutes = 30,
            Clinician = "Dr. Smith",
            Department = Department.Oncology,
            Location = "SW1A 1AA"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/appointments", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task CreateAppointment_WithInvalidData_ReturnsValidationProblem()
    {
        await CreateTestPatient();

        CreateAppointmentRequest request = new()
        {
            PatientNhsNumber = "9434765919",
            Status = AppointmentStatus.Active,
            Time = DateTimeOffset.Now.AddDays(1),
            DurationMinutes = 0, // Invalid
            Clinician = "", // Invalid
            Department = Department.Oncology,
            Location = "INVALID" // Invalid
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/appointments", request);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);

        HttpValidationProblemDetails?
            problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        await Assert.That(problem).IsNotNull();
        await Assert.That(problem!.Errors.Count).IsGreaterThanOrEqualTo(3);
    }

    [Test]
    public async Task GetAppointment_WhenExists_ReturnsAppointment()
    {
        await CreateTestPatient();

        CreateAppointmentRequest createRequest = new()
        {
            PatientNhsNumber = "9434765919",
            Status = AppointmentStatus.Active,
            Time = DateTimeOffset.Now.AddDays(1),
            DurationMinutes = 30,
            Clinician = "Dr. Smith",
            Department = Department.Oncology,
            Location = "SW1A 1AA"
        };
        HttpResponseMessage createResponse = await _client.PostAsJsonAsync("/api/appointments", createRequest);
        AppointmentResponse? created = await createResponse.Content.ReadFromJsonAsync<AppointmentResponse>(JsonOptions);

        HttpResponseMessage response = await _client.GetAsync($"/api/appointments/{created!.Id}");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        AppointmentResponse? appointment = await response.Content.ReadFromJsonAsync<AppointmentResponse>(JsonOptions);
        await Assert.That(appointment).IsNotNull();
        await Assert.That(appointment!.Id).IsEqualTo(created.Id);
    }

    [Test]
    public async Task GetAppointment_WhenNotExists_ReturnsNotFound()
    {
        HttpResponseMessage response = await _client.GetAsync($"/api/appointments/{Guid.NewGuid()}");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task UpdateAppointment_WithValidData_ReturnsOk()
    {
        await CreateTestPatient();

        CreateAppointmentRequest createRequest = new()
        {
            PatientNhsNumber = "9434765919",
            Status = AppointmentStatus.Active,
            Time = DateTimeOffset.Now.AddDays(1),
            DurationMinutes = 30,
            Clinician = "Dr. Smith",
            Department = Department.Oncology,
            Location = "SW1A 1AA"
        };
        HttpResponseMessage createResponse = await _client.PostAsJsonAsync("/api/appointments", createRequest);
        AppointmentResponse? created = await createResponse.Content.ReadFromJsonAsync<AppointmentResponse>(JsonOptions);

        UpdateAppointmentRequest updateRequest = new()
        {
            Status = AppointmentStatus.Attended,
            Time = DateTimeOffset.Now.AddDays(2),
            DurationMinutes = 45,
            Clinician = "Dr. Jones",
            Department = Department.Paediatrics,
            Location = "EC1A 1BB"
        };

        HttpResponseMessage response = await _client.PutAsJsonAsync($"/api/appointments/{created!.Id}", updateRequest);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        AppointmentResponse? appointment = await response.Content.ReadFromJsonAsync<AppointmentResponse>(JsonOptions);
        await Assert.That(appointment).IsNotNull();
        await Assert.That(appointment!.Status).IsEqualTo(AppointmentStatus.Attended);
        await Assert.That(appointment.Clinician).IsEqualTo("Dr. Jones");
        await Assert.That(appointment.DurationMinutes).IsEqualTo(45);
    }

    [Test]
    public async Task UpdateAppointment_WhenNotExists_ReturnsNotFound()
    {
        UpdateAppointmentRequest updateRequest = new()
        {
            Status = AppointmentStatus.Attended,
            Time = DateTimeOffset.Now.AddDays(2),
            DurationMinutes = 45,
            Clinician = "Dr. Jones",
            Department = Department.Paediatrics,
            Location = "EC1A 1BB"
        };

        HttpResponseMessage response =
            await _client.PutAsJsonAsync($"/api/appointments/{Guid.NewGuid()}", updateRequest);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task UpdateAppointment_WithInvalidData_ReturnsValidationProblem()
    {
        await CreateTestPatient();

        CreateAppointmentRequest createRequest = new()
        {
            PatientNhsNumber = "9434765919",
            Status = AppointmentStatus.Active,
            Time = DateTimeOffset.Now.AddDays(1),
            DurationMinutes = 30,
            Clinician = "Dr. Smith",
            Department = Department.Oncology,
            Location = "SW1A 1AA"
        };
        HttpResponseMessage createResponse = await _client.PostAsJsonAsync("/api/appointments", createRequest);
        AppointmentResponse? created = await createResponse.Content.ReadFromJsonAsync<AppointmentResponse>(JsonOptions);

        UpdateAppointmentRequest updateRequest = new()
        {
            Status = AppointmentStatus.Attended,
            Time = DateTimeOffset.Now.AddDays(2),
            DurationMinutes = -1,
            Clinician = "",
            Department = Department.Paediatrics,
            Location = "INVALID"
        };

        HttpResponseMessage response = await _client.PutAsJsonAsync($"/api/appointments/{created!.Id}", updateRequest);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task DeleteAppointment_WhenExists_ReturnsNoContent()
    {
        await CreateTestPatient();

        CreateAppointmentRequest createRequest = new()
        {
            PatientNhsNumber = "9434765919",
            Status = AppointmentStatus.Active,
            Time = DateTimeOffset.Now.AddDays(1),
            DurationMinutes = 30,
            Clinician = "Dr. Smith",
            Department = Department.Oncology,
            Location = "SW1A 1AA"
        };
        HttpResponseMessage createResponse = await _client.PostAsJsonAsync("/api/appointments", createRequest);
        AppointmentResponse? created = await createResponse.Content.ReadFromJsonAsync<AppointmentResponse>(JsonOptions);

        HttpResponseMessage response = await _client.DeleteAsync($"/api/appointments/{created!.Id}");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);

        HttpResponseMessage getResponse = await _client.GetAsync($"/api/appointments/{created.Id}");
        await Assert.That(getResponse.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeleteAppointment_WhenNotExists_ReturnsNotFound()
    {
        HttpResponseMessage response = await _client.DeleteAsync($"/api/appointments/{Guid.NewGuid()}");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetAllAppointments_AfterCreating_ReturnsAppointments()
    {
        await CreateTestPatient();

        CreateAppointmentRequest request1 = new()
        {
            PatientNhsNumber = "9434765919",
            Status = AppointmentStatus.Active,
            Time = DateTimeOffset.Now.AddDays(1),
            DurationMinutes = 30,
            Clinician = "Dr. Smith",
            Department = Department.Oncology,
            Location = "SW1A 1AA"
        };
        CreateAppointmentRequest request2 = new()
        {
            PatientNhsNumber = "9434765919",
            Status = AppointmentStatus.Active,
            Time = DateTimeOffset.Now.AddDays(2),
            DurationMinutes = 45,
            Clinician = "Dr. Jones",
            Department = Department.Paediatrics,
            Location = "EC1A 1BB"
        };
        await _client.PostAsJsonAsync("/api/appointments", request1);
        await _client.PostAsJsonAsync("/api/appointments", request2);

        HttpResponseMessage response = await _client.GetAsync("/api/appointments");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        List<AppointmentResponse>? appointments = await response.Content.ReadFromJsonAsync<List<AppointmentResponse>>(JsonOptions);
        await Assert.That(appointments).IsNotNull();
        await Assert.That(appointments!.Count).IsEqualTo(2);
    }
}