using AireTechTest.Server.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add PostgreSQL database context (skipped when UseInMemoryDatabase is set for testing)
if (!builder.Configuration.GetValue<bool>("UseInMemoryDatabase"))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        string connectionString = builder.Configuration.GetConnectionString("postgresdb")
            ?? throw new InvalidOperationException("Connection string 'postgresdb' not found.");
        options.UseNpgsql(connectionString);
    });
}

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Apply pending migrations automatically in development (skip for in-memory test database)
if (app.Environment.IsDevelopment() && !app.Configuration.GetValue<bool>("UseInMemoryDatabase"))
{
    using IServiceScope scope = app.Services.CreateScope();
    ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();

app.Run();

// Make Program accessible for integration tests
public partial class Program { }
