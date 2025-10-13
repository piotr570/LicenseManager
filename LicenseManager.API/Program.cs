using LicenseManager.Application;
using LicenseManager.Application.Middlewares;
using LicenseManager.Controllers;
using LicenseManager.Domain;
using LicenseManager.Infrastructure;
using LicenseManager.Notification.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

ConfigureLogging(builder);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDomainLayer();
builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureLayer(builder.Configuration);

// Module registrations
builder.Services.AddNotificationModule();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandling();
app.UseRequestLogging();

app.MapControllers();
app.MapLicenseEndpoints();
app.MapUserEndpoints();
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