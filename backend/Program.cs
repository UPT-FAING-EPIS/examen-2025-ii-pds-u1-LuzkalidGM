// Project Management API - Sistema de gestión de proyectos
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjectManagement.Api.Data;
using ProjectManagement.Api.Configuration;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Database configuration - Azure SQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? DatabaseConfig.GetConnectionStringFromEnvironment();

builder.Services.AddDbContext<ProjectManagementContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null);
    }));

// JWT Authentication configuration
var jwtKey = builder.Configuration["Jwt:Key"] ?? "default-secret-key-for-development-only";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ProjectManagement.Api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ProjectManagement.Client";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Para desarrollo
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Application services
builder.Services.AddScoped<ProjectManagement.Api.Services.IUserService, ProjectManagement.Api.Services.UserService>();
builder.Services.AddScoped<ProjectManagement.Api.Services.IProjectService, ProjectManagement.Api.Services.ProjectService>();
builder.Services.AddScoped<ProjectManagement.Api.Services.ITaskService, ProjectManagement.Api.Services.TaskService>();
builder.Services.AddScoped<ProjectManagement.Api.Services.IJwtService, ProjectManagement.Api.Services.JwtService>();

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",  // React dev server
            "http://localhost:5173",  // Vite dev server
            "https://localhost:3000",
            "https://localhost:5173"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ProjectManagementContext>();

// Swagger/OpenAPI configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Project Management API",
        Version = "v1",
        Description = "API para gestión de proyectos y tareas",
        Contact = new OpenApiContact
        {
            Name = "Development Team",
            Email = "dev@projectmanagement.com"
        }
    });

    // JWT Bearer authorization in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
            Array.Empty<string>()
        }
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Logging configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Management API V1");
        c.RoutePrefix = string.Empty; // Swagger como página de inicio
    });
}

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    await next();
});

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// Health check endpoints
app.MapGet("/", () => "Project Management API is running! 🚀");
app.MapGet("/health", () => new { 
    status = "Healthy", 
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
});

app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health");

// Database migration and seeding (only in development)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ProjectManagementContext>();
    
    try
    {
        // Test database connection
        await context.Database.CanConnectAsync();
        app.Logger.LogInformation("Database connection verified successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error connecting to database");
    }
}

app.Logger.LogInformation("Project Management API started successfully");
app.Logger.LogInformation("Database: {ConnectionString}", connectionString.Split(';')[0] + ";...");

app.Run();