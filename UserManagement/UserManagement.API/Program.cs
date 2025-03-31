using MediatR;
using Microsoft.OpenApi.Models;
using UserManagement.API.Extensions;
using UserManagement.API.Middlewares;
using UserManagement.Application.Common.Mappings;
using UserManagement.Application.Common.Validation;
using UserManagement.Domain.Interfaces.IServices;
using UserManagement.Infrastructure.Persistence.Context;
using UserManagement.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddApplicationServices();
builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddValidationServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCustomMiddlewares();
builder.Services.AddScoped<IEmailConfirmationService, EmailConfirmationService>();
builder.Services.AddScoped<IEmailDeliveryService, EmailDeliveryService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationService<,>));

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите ваш Bearer токен. Пример: 'abcdef12345'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddHttpClient<IConnectionService, ConnectionService>(client =>
{
    var baseUrl = builder.Configuration["ProductManagementApi:BaseUrl"];
    if (string.IsNullOrEmpty(baseUrl))
        throw new ArgumentNullException(nameof(baseUrl), "Базовый URL для ProductManagementApi не задан в конфигурации.");

    client.BaseAddress = new Uri(baseUrl);
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy => policy.WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowProductService",
        policy => policy.WithOrigins("http://localhost:5019") 
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});

var app = builder.Build();
app.UseCors("AllowReact");
app.UseCors("AllowProductService");
app.UseMiddleware<ValidationMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
await UserManagementDbContextInitializer.InitializeAsync(app.Services);
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
