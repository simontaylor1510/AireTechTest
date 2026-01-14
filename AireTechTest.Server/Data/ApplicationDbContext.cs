using AireTechTest.Server.Domain;

using Microsoft.EntityFrameworkCore;

namespace AireTechTest.Server.Data;

/// <summary>
/// Application database context for managing patients and appointments.
/// </summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the patients table.
    /// </summary>
    public DbSet<Patient> Patients => Set<Patient>();

    /// <summary>
    /// Gets or sets the appointments table.
    /// </summary>
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigurePatient(modelBuilder);
        ConfigureAppointment(modelBuilder);
    }

    private static void ConfigurePatient(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("patients");

            // NHS Number is the primary key
            entity.HasKey(p => p.NhsNumber);

            entity.Property(p => p.NhsNumber)
                .HasColumnName("nhs_number")
                .HasMaxLength(10)
                .HasConversion<NhsNumberConverter>()
                .IsRequired();

            entity.Property(p => p.Name)
                .HasColumnName("name")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(p => p.DateOfBirth)
                .HasColumnName("date_of_birth")
                .IsRequired();

            entity.Property(p => p.Postcode)
                .HasColumnName("postcode")
                .HasMaxLength(8)
                .HasConversion<PostcodeConverter>()
                .IsRequired();

            // Ignore computed property
            entity.Ignore(p => p.Age);
        });
    }

    private static void ConfigureAppointment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("appointments");

            entity.HasKey(a => a.Id);

            entity.Property(a => a.Id)
                .HasColumnName("id")
                .HasConversion<AppointmentIdConverter>()
                .IsRequired();

            entity.Property(a => a.PatientNhsNumber)
                .HasColumnName("patient_nhs_number")
                .HasMaxLength(10)
                .HasConversion<NhsNumberConverter>()
                .IsRequired();

            entity.Property(a => a.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(a => a.Time)
                .HasColumnName("time")
                .IsRequired();

            entity.Property(a => a.Duration)
                .HasColumnName("duration")
                .IsRequired();

            entity.Property(a => a.Clinician)
                .HasColumnName("clinician")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(a => a.Department)
                .HasColumnName("department")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(a => a.Location)
                .HasColumnName("location_postcode")
                .HasMaxLength(8)
                .HasConversion<PostcodeConverter>()
                .IsRequired();

            // Ignore computed properties
            entity.Ignore(a => a.EndTime);
            entity.Ignore(a => a.IsPast);
            entity.Ignore(a => a.IsFuture);

            // Foreign key relationship to Patient
            entity.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(a => a.PatientNhsNumber)
                .OnDelete(DeleteBehavior.Cascade);

            // Index on patient for efficient queries
            entity.HasIndex(a => a.PatientNhsNumber)
                .HasDatabaseName("ix_appointments_patient_nhs_number");

            // Index on time for date-range queries
            entity.HasIndex(a => a.Time)
                .HasDatabaseName("ix_appointments_time");

            // Index on status for filtering
            entity.HasIndex(a => a.Status)
                .HasDatabaseName("ix_appointments_status");
        });
    }
}