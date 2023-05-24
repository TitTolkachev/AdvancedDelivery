using AutoMapper;
using Backend.BL.Services;
using Backend.BL.Services.ValidateTokenPolicy;
using Backend.Common.Interfaces;
using Backend.Common.Mappings;
using Backend.DAL;
using Common.Configuration;
using Common.Middleware.ExceptionHandler;
using DeliveryBackend.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Backend.Api",
        Version = "v1",
        Description = "Backend API"
    });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

// DB
var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connection));

// Rabbit
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();
builder.Services.AddScoped<IProducerService, ProducerService>();

// AutoMapping
var mapperConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddMvc();

// Services
builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<ICookService, CookService>();
builder.Services.AddScoped<IManagerService, ManagerService>();
builder.Services.AddScoped<ICourierService, CourierService>();

// Auth
// builder.Services.AddSingleton<IAuthorizationHandler, ValidateTokenRequirementHandler>();
// builder.Services.AddHttpContextAccessor();
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidIssuer = JwtConfigurations.Issuer,
//             ValidateAudience = true,
//             ValidAudience = JwtConfigurations.Audience,
//             ValidateLifetime = true,
//             IssuerSigningKey = JwtConfigurations.GetSymmetricSecurityKey(),
//             ValidateIssuerSigningKey = true,
//         };
//     });
// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy(
//         "ValidateToken",
//         policy => policy.Requirements.Add(new ValidateTokenRequirement()));
// });

builder.ConfigureJwt();

// TODO(Перенести в Auth)
// Quartz
// builder.Services.AddQuartz(q =>
// {
//     q.UseMicrosoftDependencyInjectionJobFactory();
//     var jobKey = new JobKey("DeleteInvalidTokensJob");
//     q.AddJob<DeleteInvalidTokensJob>(opts => opts.WithIdentity(jobKey));
//     q.AddTrigger(opts => opts
//         .ForJob(jobKey)
//         .WithIdentity("DeleteInvalidTokensJob-trigger")
//         .WithCronSchedule("0 0 0 ? * *")
//     );
// });
// builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Build App
var app = builder.Build();

// TODO(Включить)
// Middleware Exceptions
app.UseExceptionHandlerMiddleware();

// Auto Migration
using var serviceScope = app.Services.CreateScope();
var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
context?.Database.Migrate();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();