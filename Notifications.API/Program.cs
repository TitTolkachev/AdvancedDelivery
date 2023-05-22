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

// SignalR
builder.Services.AddSingleton<NotificationsHub>();
builder.Services.AddSignalR();

// Rabbit
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();
builder.Services.AddSingleton<IConsumerService, ConsumerService>();
builder.Services.AddHostedService<ConsumerHostedService>();

// Services
builder.Services.AddSingleton<INotificationsService, NotificationsService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.EnableAnnotations();
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Notifications", Version = "v1" });
});

// Cors
const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(myAllowSpecificOrigins,
        policy =>
        {
            policy.SetIsOriginAllowed(_ => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

var app = builder.Build();

// Middleware Exceptions
//app.UseExceptionHandlerMiddleware();

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

//app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(myAllowSpecificOrigins);

app.MapControllers();

app.MapHub<NotificationsHub>("/notifications");

app.Run();