using Common.Middleware.ExceptionHandler;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Notifications.BL.Hubs;
using Notifications.BL.Services;
using Notifications.Common.Interfaces;
using Notifications.DAL;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Db Context
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Db")));

// Services
builder.Services.AddScoped<INotificationsService, NotificationsService>();

// SignalR
builder.Services.AddSignalR();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.EnableAnnotations();
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Notifications", Version = "v1" });
});

var app = builder.Build();

// Middleware Exceptions
app.UseExceptionHandlerMiddleware();

// Auto Migration
using var serviceScope = app.Services.CreateScope();
var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
context?.Database.Migrate();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationsHub>("/notifications");

app.Run();