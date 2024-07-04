using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;
using notification.api;
using notification.bll;

var builder = WebApplication.CreateBuilder(args);



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
    return new NotificationReceiver("localhost", "guest", "guest", telemetryClient);
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
