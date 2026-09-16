using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Practical_Test_Xtramile.Application.Common.Behaviors;
using Practical_Test_Xtramile.Application.Common.Interfaces;
using Practical_Test_Xtramile.Infrastructure.Persistence;
using Practical_Test_Xtramile.Infrastructure.Persistence.Seed;
using Practical_Test_Xtramile.Infrastructure.Services;
using Practical_Test_Xtramile.Infrastructure.WeatherApi;
using Practical_Test_Xtramile.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Controllers and JSON options
builder.Services.AddControllers();

// 2. Configure EF Core SQLite / In-Memory (Environment Gated)
if (builder.Environment.IsEnvironment("Testing") ||
    builder.Configuration.GetValue<string>("Database:Provider")?.Equals("InMemory", StringComparison.OrdinalIgnoreCase) == true)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseInMemoryDatabase("WeatherTestDb");
    });
}
else
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                              ?? "Data Source=App_Data/weather.db";

    // Ensure App_Data folder exists for SQLite file
    if (connectionString.Contains("App_Data", StringComparison.OrdinalIgnoreCase))
    {
        Directory.CreateDirectory("App_Data");
    }

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlite(connectionString);
    });
}

builder.Services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

// 3. Configure MediatR with Pipeline Validation Behavior
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// 4. Configure FluentValidation validators
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// 5. Configure Weather API & Core Infrastructure Services
builder.Services.Configure<OpenWeatherMapSettings>(
    builder.Configuration.GetSection(OpenWeatherMapSettings.SectionName));

builder.Services.AddHttpClient<IWeatherService, OpenWeatherMapService>();
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

// 6. OpenAPI & Swagger
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 7. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 8. Global Exception Handling Middleware (RFC 9110 ProblemDetails)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 9. Static Files & Frontend Client
app.UseDefaultFiles();
app.UseStaticFiles();

// 10. CORS & Routing
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Xtramile Weather API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseAuthorization();
app.MapControllers();

// 11. Database Initialization & Master Data Seeding (Environment Gated)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var config = services.GetRequiredService<IConfiguration>();
    var env = services.GetRequiredService<IWebHostEnvironment>();

    bool shouldSeed = env.IsEnvironment("Testing") ||
                      config.GetValue<bool>("Database:SeedData", defaultValue: env.IsDevelopment());

    if (shouldSeed)
    {
        await context.Database.EnsureCreatedAsync();
        await MasterDataSeeder.SeedAsync(context);
    }
}

app.Run();

// Required for WebApplicationFactory in integration tests
public partial class Program { }
