using Serilog;
using LicenseManager.Licenses;
using LicenseManager.Licenses.API;
using LicenseManager.Middlewares;
using LicenseManager.Users;
using LicenseManager.Users.API;
using LicenseManager.Notification.Infrastructure;
using LicenseManager.Observability;

var builder = WebApplication.CreateBuilder(args);

ConfigureLogging(builder);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       builder.Configuration["DatabaseConfiguration:ConnectionString"]!;

builder.Services.AddLicensesModule(connectionString);
builder.Services.AddUsersModule(connectionString);
builder.Services.AddObservabilityModule();
builder.Services.AddNotificationModule();

builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policy =>
    {
        policy.WithOrigins("*", "http://localhost:62212")
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandling();
app.UseRequestLogging();

app.MapLicensesModule();
app.MapUsersModule();
app.MapObservabilityModuleEndpoints();
app.MapNotificationModuleEndpoints();

app.Run();

void ConfigureLogging(WebApplicationBuilder webApplicationBuilder)
{
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(webApplicationBuilder.Configuration)
        .Enrich.FromLogContext()
        .CreateLogger();
    webApplicationBuilder.Host.UseSerilog();
}

public partial class Program { }