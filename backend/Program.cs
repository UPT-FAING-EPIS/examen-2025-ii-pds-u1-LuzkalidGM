using Microsoft.AspNetCore.Authentication.JwtBearer;// Project Management API - Sistema de gestión de proyectos

using Microsoft.EntityFrameworkCore;using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.IdentityModel.Tokens;using Microsoft.EntityFrameworkCore;

using Microsoft.OpenApi.Models;using Microsoft.IdentityModel.Tokens;

using ProjectManagement.Api.Data;using Microsoft.OpenApi.Models;

using ProjectManagement.Api.Configuration;using ProjectManagement.Api.Data;

using System.Reflection;using ProjectManagement.Api.Configuration;

using System.Text;using System.Reflection;

using System.Text.Json.Serialization;using System.Text;

using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()// Add services to the container.

    .AddJsonOptions(options =>builder.Services.AddControllers()

    {    .AddJsonOptions(options =>

        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());    {

        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    });        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

    });

// Database configuration - Azure SQL

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") // Database configuration - Azure SQL

    ?? DatabaseConfig.GetConnectionStringFromEnvironment();var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 

    ?? DatabaseConfig.GetConnectionStringFromEnvironment();

builder.Services.AddDbContext<ProjectManagementContext>(options =>

    options.UseSqlServer(connectionString, sqlOptions =>builder.Services.AddDbContext<ProjectManagementContext>(options =>

    {    options.UseSqlServer(connectionString, sqlOptions =>

        sqlOptions.EnableRetryOnFailure(    {

            maxRetryCount: 3,        sqlOptions.EnableRetryOnFailure(

            maxRetryDelay: TimeSpan.FromSeconds(5),            maxRetryCount: 3,

            errorNumbersToAdd: null);            maxRetryDelay: TimeSpan.FromSeconds(5),

    }));            errorNumbersToAdd: null);

    }));

// JWT Authentication configuration

var jwtKey = builder.Configuration["Jwt:Key"] ?? "default-secret-key-for-development-only";// JWT Authentication configuration

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ProjectManagement.Api";var jwtKey = builder.Configuration["Jwt:Key"] ?? "default-secret-key-for-development-only";

var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ProjectManagement.Client";var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ProjectManagement.Api";

var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ProjectManagement.Client";

builder.Services.AddAuthentication(options =>

{builder.Services.AddAuthentication(options =>

    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;{

    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

})    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

.AddJwtBearer(options =>})

{.AddJwtBearer(options =>

    options.RequireHttpsMetadata = false;{

    options.SaveToken = true;    options.RequireHttpsMetadata = false; // Para desarrollo

    options.TokenValidationParameters = new TokenValidationParameters    options.SaveToken = true;

    {    options.TokenValidationParameters = new TokenValidationParameters

        ValidateIssuerSigningKey = true,    {

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),        ValidateIssuerSigningKey = true,

        ValidateIssuer = true,        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

        ValidIssuer = jwtIssuer,        ValidateIssuer = true,

        ValidateAudience = true,        ValidIssuer = jwtIssuer,

        ValidAudience = jwtAudience,        ValidateAudience = true,

        ValidateLifetime = true,        ValidAudience = jwtAudience,

        ClockSkew = TimeSpan.Zero        ValidateLifetime = true,

    };        ClockSkew = TimeSpan.Zero

});    };

});

builder.Services.AddAuthorization();

builder.Services.AddAuthorization();

// CORS configuration

builder.Services.AddCors(options =>// CORS configuration

{builder.Services.AddCors(options =>

    options.AddPolicy("AllowFrontend", policy =>{

    {    options.AddPolicy("AllowFrontend", policy =>

        policy.WithOrigins(    {

            "http://localhost:3000",        policy.WithOrigins(

            "http://localhost:5173",            "http://localhost:3000",  // React dev server

            "https://localhost:3000",            "http://localhost:5173",  // Vite dev server

            "https://localhost:5173"            "https://localhost:3000",

        )            "https://localhost:5173"

        .AllowAnyHeader()        )

        .AllowAnyMethod()        .AllowAnyHeader()

        .AllowCredentials();        .AllowAnyMethod()

    });        .AllowCredentials();

});    });

});

// Health checks

builder.Services.AddHealthChecks()// Health checks

    .AddEntityFrameworkCore<ProjectManagementContext>();builder.Services.AddHealthChecks()

    .AddDbContext<ProjectManagementContext>();

// Swagger/OpenAPI

builder.Services.AddEndpointsApiExplorer();// Swagger/OpenAPI configuration

builder.Services.AddSwaggerGen(c =>builder.Services.AddEndpointsApiExplorer();

{builder.Services.AddSwaggerGen(c =>

    c.SwaggerDoc("v1", new OpenApiInfo{

    {    c.SwaggerDoc("v1", new OpenApiInfo

        Title = "Project Management API",    {

        Version = "v1",        Title = "Project Management API",

        Description = "API para gestión de proyectos y tareas"        Version = "v1",

    });        Description = "API para gestión de proyectos y tareas",

        Contact = new OpenApiContact

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme        {

    {            Name = "Development Team",

        Description = "JWT Authorization header using the Bearer scheme.",            Email = "dev@projectmanagement.com"

        Name = "Authorization",        }

        In = ParameterLocation.Header,    });

        Type = SecuritySchemeType.ApiKey,

        Scheme = "Bearer"    // JWT Bearer authorization in Swagger

    });    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme

    {

    c.AddSecurityRequirement(new OpenApiSecurityRequirement        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",

    {        Name = "Authorization",

        {        In = ParameterLocation.Header,

            new OpenApiSecurityScheme        Type = SecuritySchemeType.ApiKey,

            {        Scheme = "Bearer"

                Reference = new OpenApiReference    });

                {

                    Type = ReferenceType.SecurityScheme,    c.AddSecurityRequirement(new OpenApiSecurityRequirement

                    Id = "Bearer"    {

                }        {

            },            new OpenApiSecurityScheme

            Array.Empty<string>()            {

        }                Reference = new OpenApiReference

    });                {

                    Type = ReferenceType.SecurityScheme,

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";                    Id = "Bearer"

    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);                }

    if (File.Exists(xmlPath))            },

    {            Array.Empty<string>()

        c.IncludeXmlComments(xmlPath);        }

    }    });

});

    // Include XML comments

var app = builder.Build();    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

// Configure the HTTP request pipeline.    if (File.Exists(xmlPath))

if (app.Environment.IsDevelopment())    {

{        c.IncludeXmlComments(xmlPath);

    app.UseSwagger();    }

    app.UseSwaggerUI(c =>});

    {

        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Management API V1");// Logging configuration

        c.RoutePrefix = string.Empty;builder.Logging.ClearProviders();

    });builder.Logging.AddConsole();

}builder.Logging.AddDebug();



app.Use(async (context, next) =>var app = builder.Build();

{

    context.Response.Headers["X-Content-Type-Options"] = "nosniff";// Configure the HTTP request pipeline.

    context.Response.Headers["X-Frame-Options"] = "DENY";if (app.Environment.IsDevelopment())

    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";{

    await next();    app.UseSwagger();

});    app.UseSwaggerUI(c =>

    {

app.UseHttpsRedirection();        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Management API V1");

app.UseCors("AllowFrontend");        c.RoutePrefix = string.Empty; // Swagger como página de inicio

app.UseAuthentication();    });

app.UseAuthorization();}

app.MapControllers();

app.MapHealthChecks("/health");// Security headers

app.Use(async (context, next) =>

// Database initialization{

if (app.Environment.IsDevelopment())    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

{    context.Response.Headers.Add("X-Frame-Options", "DENY");

    using var scope = app.Services.CreateScope();    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");

    var context = scope.ServiceProvider.GetRequiredService<ProjectManagementContext>();    await next();

    });

    try

    {app.UseHttpsRedirection();

        await context.Database.EnsureCreatedAsync();

        app.Logger.LogInformation("Database connection verified");app.UseCors("AllowFrontend");

    }

    catch (Exception ex)app.UseAuthentication();

    {app.UseAuthorization();

        app.Logger.LogError(ex, "Database connection error");

    }app.MapControllers();

}

// Health check endpoint

app.Logger.LogInformation("Project Management API started");app.MapHealthChecks("/health");

app.Run();
// Database migration and seeding (only in development)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ProjectManagementContext>();
    
    try
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();
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
        ValidAudience = jwtSettings["Audience"] ?? "ProjectManagement.Api",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Authorization
builder.Services.AddAuthorization();

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Application services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// API Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Project Management API",
        Version = "v1",
        Description = "API para la gestión integral de proyectos y actividades",
        Contact = new OpenApiContact
        {
            Name = "Equipo de Desarrollo",
            Email = "dev@projectmanagement.com"
        }
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // JWT Authentication in Swagger
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
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ProjectManagementContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Management API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at app's root
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

// Database migration and seeding
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ProjectManagementContext>();
        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.Run();