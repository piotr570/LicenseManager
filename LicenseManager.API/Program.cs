using LicenseManager.Application;
using LicenseManager.Application.Middlewares;
using LicenseManager.Domain;
using LicenseManager.Infrastructure;
using LicenseManager.Modules;
using LicenseManager.Notification.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

ConfigureLogging(builder);

var mvcBuilder = builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDomainLayer();
builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureLayer(builder.Configuration);

// Module registrations
builder.Services.AddNotificationModule(mvcBuilder);

builder.Services.AddCors(options =>
{
    // this defines a CORS policy called "default"
    options.AddPolicy("default", policy =>
    {
        policy.WithOrigins("*", "http://localhost:62212")
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

ApplyMigrations(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandling();
app.UseRequestLogging();

app.AddLicenseModule();
app.AddUserModule();

app.UseAuthorization();
app.MapControllers();

app.Run();

void ConfigureLogging(WebApplicationBuilder webApplicationBuilder)
{
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(webApplicationBuilder.Configuration)
        .Enrich.FromLogContext()
        .CreateLogger();
    webApplicationBuilder.Host.UseSerilog();
}

void ApplyMigrations(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var serviceProvider = scope.ServiceProvider;

    try
    {
        var dbContext = serviceProvider.GetRequiredService<LicenseManagerDbContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "An error occurred while applying migrations");
        throw;
    }
}

public partial class Program { }