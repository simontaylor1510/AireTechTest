using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AireTechTest.Server.Data;

/// <summary>
/// Factory for creating ApplicationDbContext at design time for EF Core migrations.
/// The connection string here is only used by EF Core tools (dotnet ef migrations add, etc.)
/// and is not used at runtime where Aspire provides the connection string.
/// </summary>
[SuppressMessage("ReSharper", "UnusedType.Global")]
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();

        // This connection string is only used for design-time operations (migrations)
        // At runtime, Aspire provides the actual connection string
        optionsBuilder.UseNpgsql("Host=localhost;Database=airetechtest;Username=postgres;Password=postgres");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}