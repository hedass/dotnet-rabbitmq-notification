using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;
using notification.api;
using notification.bll;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(80); // Default HTTP port
    serverOptions.ListenAnyIP(443); // Another port if needed
});

builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("AZURE_APPLICATIONINSIGHTS_CONNECTIONSTRING");
});

// Register TelemetryClient as a singleton
builder.Services.AddSingleton<ITelemetryInitializer, OperationCorrelationTelemetryInitializer>();
builder.Services.AddSingleton<TelemetryClient>();

// Add services to the container.
builder.Services.AddSingleton<NotificationReceiver>((sp) =>
{
    var telemetryClient = sp.GetRequiredService<TelemetryClient>();


    var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
    Console.WriteLine(rabbitMqHost);
    return new NotificationReceiver(rabbitMqHost, "guest", "guest", telemetryClient);
});

builder.Services.AddSingleton<INotificationService, NotificationService>();
builder.Services.AddHostedService<MyBackgroundService>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
