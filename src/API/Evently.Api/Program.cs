using Evently.Api.Extensions;
using Evently.Api.Middlewares;
using Evently.Common.Application;
using Evently.Common.Infrastructure;
using Evently.Common.Presentation.Endpoints;
using Evently.Modules.Attendance.Infrastructure;
using Evently.Modules.Events.Infrastructure;
using Evently.Modules.Ticketing.Infrastructure;
using Evently.Modules.Users.Infrastructure;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// register configuration files
builder.Configuration.AddModuleConfiguration(["events", "users", "ticketing", "attendance"]);

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.CustomSchemaIds(t => t.FullName?.Replace("+", "."));
});

// register exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// register cross-cutting concerns
string databaseConnectionString = builder.Configuration.GetConnectionString("Database")!;
string redisConnectionString = builder.Configuration.GetConnectionString("Cache")!;
builder.Services
    .AddApplication([
    Evently.Modules.Events.Application.AssemblyMarker.Assembly,
    Evently.Modules.Users.Application.AssemblyMarker.Assembly,
    Evently.Modules.Ticketing.Application.AssemblyMarker.Assembly,
    Evently.Modules.Attendance.Application.AssemblyMarker.Assembly,
])
    .AddInfrastructure([
        AttendanceModule.ConfigureConsumers,
        TicketingModule.ConfigureConsumers
    ],
        databaseConnectionString,
        redisConnectionString
        );

// register health checks
builder.Services.AddHealthChecks()
    .AddNpgSql(databaseConnectionString)
    .AddRedis(redisConnectionString)
    .AddUrlGroup(new Uri(builder.Configuration.GetValue<string>("KeyCloak:HealthUrl")!), HttpMethod.Get, "keycloak");
    

// register specific modules
builder.Services.AddEventsModule(builder.Configuration);
builder.Services.AddUsersModule(builder.Configuration);
builder.Services.AddTicketingModule(builder.Configuration);
builder.Services.AddAttendanceModule(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
}

app.UseSerilogRequestLogging();
app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapEndpoints();

app.Run();
