using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using AireTechTest.Server.Domain;

using Microsoft.EntityFrameworkCore;

namespace AireTechTest.Server.Data;

public static partial class DatabaseSeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task SeedAsync(ApplicationDbContext dbContext, string testDataPath)
    {
        if (await dbContext.Patients.AnyAsync())
        {
            return; // Already seeded
        }

        await SeedPatientsAsync(dbContext, testDataPath);
        await SeedAppointmentsAsync(dbContext, testDataPath);
    }

    private static async Task SeedPatientsAsync(ApplicationDbContext dbContext, string testDataPath)
    {
        string filePath = Path.Combine(testDataPath, "example_patients.json");
        if (!File.Exists(filePath))
        {
            return;
        }

        string json = await File.ReadAllTextAsync(filePath);
        var patientDtos = JsonSerializer.Deserialize<List<PatientDto>>(json, JsonOptions) ?? [];

        foreach (var dto in patientDtos)
        {
            try
            {
                var patient = new Patient
                {
                    NhsNumber = NhsNumber.From(dto.NhsNumber),
                    Name = dto.Name,
                    DateOfBirth = DateOnly.Parse(dto.DateOfBirth),
                    Postcode = Postcode.From(dto.Postcode)
                };
                dbContext.Patients.Add(patient);
            }
            catch
            {
                // Skip invalid records
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedAppointmentsAsync(ApplicationDbContext dbContext, string testDataPath)
    {
        string filePath = Path.Combine(testDataPath, "example_appointments.json");
        if (!File.Exists(filePath))
        {
            return;
        }

        string json = await File.ReadAllTextAsync(filePath);
        var appointmentDtos = JsonSerializer.Deserialize<List<AppointmentDto>>(json, JsonOptions) ?? [];

        foreach (var dto in appointmentDtos)
        {
            try
            {
                var appointment = new Appointment
                {
                    Id = AppointmentId.From(Guid.Parse(dto.Id)),
                    PatientNhsNumber = NhsNumber.From(dto.Patient),
                    Status = ParseStatus(dto.Status),
                    Time = DateTimeOffset.Parse(dto.Time).ToUniversalTime(),
                    Duration = ParseDuration(dto.Duration),
                    Clinician = dto.Clinician,
                    Department = ParseDepartment(dto.Department),
                    Location = Postcode.From(dto.Postcode)
                };
                dbContext.Appointments.Add(appointment);
            }
            catch
            {
                // Skip invalid records
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private static AppointmentStatus ParseStatus(string status) =>
        status.ToLowerInvariant() switch
        {
            "active" => AppointmentStatus.Active,
            "attended" => AppointmentStatus.Attended,
            "missed" => AppointmentStatus.Missed,
            "cancelled" => AppointmentStatus.Cancelled,
            _ => AppointmentStatus.Active
        };

    private static Department ParseDepartment(string department) =>
        department.ToLowerInvariant() switch
        {
            "oncology" => Department.Oncology,
            "gastroenterology" or "gastroentology" => Department.Gastroenterology,
            "orthopaedics" => Department.Orthopaedics,
            "paediatrics" => Department.Paediatrics,
            _ => Department.Oncology
        };

    private static TimeSpan ParseDuration(string duration)
    {
        var match = DurationRegex().Match(duration);
        if (!match.Success)
        {
            return TimeSpan.FromMinutes(30);
        }

        int hours = match.Groups["hours"].Success ? int.Parse(match.Groups["hours"].Value) : 0;
        int minutes = match.Groups["minutes"].Success ? int.Parse(match.Groups["minutes"].Value) : 0;

        return TimeSpan.FromHours(hours) + TimeSpan.FromMinutes(minutes);
    }

    [GeneratedRegex(@"^(?:(?<hours>\d+)h)?(?:(?<minutes>\d+)m)?$")]
    private static partial Regex DurationRegex();

    private sealed record PatientDto
    {
        [JsonPropertyName("nhs_number")]
        public required string NhsNumber { get; init; }

        public required string Name { get; init; }

        [JsonPropertyName("date_of_birth")]
        public required string DateOfBirth { get; init; }

        public required string Postcode { get; init; }
    }

    private sealed record AppointmentDto
    {
        public required string Id { get; init; }
        public required string Patient { get; init; }
        public required string Status { get; init; }
        public required string Time { get; init; }
        public required string Duration { get; init; }
        public required string Clinician { get; init; }
        public required string Department { get; init; }
        public required string Postcode { get; init; }
    }
}
