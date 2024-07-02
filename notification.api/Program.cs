using Microsoft.Extensions.DependencyInjection.Extensions;
using notification.api;
using notification.bll;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<NotificationReceiver>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new NotificationReceiver("localhost", "guest", "guest");
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
