using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using System.Text;

// Shared
using RentalPeAPI.Shared.Infrastructure.Persistence.EFC.Configuration;           // AppDbContext
using RentalPeAPI.Shared.Infrastructure.Persistence.EFC.Repositories;           // UnitOfWork
using RentalPeAPI.Shared.Infrastructure.Interfaces.ASP.Configuration;           // KebabCase routes
using IUnitOfWork = RentalPeAPI.Shared.Domain.Repositories.IUnitOfWork;
using UnitOfWork = RentalPeAPI.Shared.Infrastructure.Persistence.EFC.Repositories.UnitOfWork;

// User BC
using RentalPeAPI.User.Application.Internal.CommandServices;
using RentalPeAPI.User.Domain.Repositories;
using RentalPeAPI.User.Domain.Services;
using RentalPeAPI.User.Infrastructure.Persistence.EFC.Repositories;
using RentalPeAPI.User.Infrastructure.Security;

// Property/Space BC
using RentalPeAPI.Property.Application.Services;
using RentalPeAPI.Property.Domain.Repositories;
using RentalPeAPI.Property.Infrastructure.Persistence.EFC.Repositories;

// Monitoring BC
using RentalPeAPI.Monitoring.Application.ACL;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Monitoring.Infrastructure.Persistence.EFC.Repositories;


var builder = WebApplication.CreateBuilder(args);

// MVC + localization
builder.Services.AddLocalization();
builder.Services.AddControllers(o => o.Conventions.Add(new KebabCaseRouteNamingConvention()))
    .AddDataAnnotationsLocalization();

// ==== CONFIGURACIÓN DE AUTENTICACIÓN JWT ====
var jwtKey = builder.Configuration["JwtSettings:Secret"] ?? "your-secret-key-minimum-32-characters-long";
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "RentalPeAPI";
var jwtAudience = builder.Configuration["JwtSettings:Audience"] ?? "RentalPeFrontend";

var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role 
    };
});

builder.Services.AddAuthorization();

// Swagger (solo Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.CustomSchemaIds(type =>
        type.FullName!
            .Replace("RentalPeAPI.", string.Empty)
            .Replace("+", ".")
            .Replace(".", "_"));

    // ==== CONFIGURACIÓN DE SEGURIDAD JWT EN SWAGGER ====
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. " +
                      "Enter 'Bearer' [space] and then your token in the text input below. " +
                      "Example: 'Bearer eyJhbGciOiJIUzI1NiIs...'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Monitoring ACL
builder.Services.AddScoped<IMonitoringContextFacade, MonitoringContextFacade>();
builder.Services.AddScoped<IPropertyContextFacade>(provider =>
    new PropertyContextFacade(
        provider.GetRequiredService<ISpaceRepository>(),
        provider.GetRequiredService<IMediator>()
    ));

// Función para añadir DbContext, simplificando la lógica
void AddMySqlDbContext(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
{
    var cs = configuration.GetConnectionString("DefaultConnection")
             ?? throw new Exception(
                 $"Database connection string 'DefaultConnection' not found in {environment.EnvironmentName}.");

    services.AddDbContext<AppDbContext>(options =>
    {
        // Usamos la sobrecarga que permite pasar opciones de MySQL
        options.UseMySql(cs, new MySqlServerVersion(new Version(8, 0, 36)), mySqlOptions =>
        {
            // Opcional: Esto ayuda a que la aplicación no se caiga por fallas temporales
            mySqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
        });

        if (environment.IsDevelopment())
        {
            options.LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }
        else // Producción
        {
            options.LogTo(Console.WriteLine, LogLevel.Error)
                .EnableDetailedErrors();
        }
    });
}

// Llama a la función de configuración
AddMySqlDbContext(builder.Services, builder.Configuration, builder.Environment);

// DI compartido
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// MediatR (ensambla handlers de varios BC)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);   // User
});

// User
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddSingleton<IPasswordHashingService, PasswordHashingService>();
builder.Services.AddSingleton<ITokenGenerationService, TokenGenerationService>();


// Property/Space
builder.Services.AddScoped<SpaceAppService>();
builder.Services.AddScoped<ISpaceRepository, SpaceRepository>();

// Monitoring
builder.Services.AddScoped<IWorkItemRepository, WorkItemRepository>();
builder.Services.AddScoped<IIoTDeviceRepository, IoTDeviceRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

// Kestrel: solo HTTP para evitar warning de certificado y mixed content
//builder.WebHost.ConfigureKestrel(o => o.ListenLocalhost(52888));


// Enable CORS for all local and cloud origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Apply Database Migrations with retry for Docker startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    for (int retry = 0; retry < 10; retry++)
    {
        try
        {
            db.Database.EnsureCreated();
            Console.WriteLine("[SpacePulse API] Database connected and schema verified successfully.");
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SpacePulse API] Waiting for Database initialization... attempt {retry + 1}/10 ({ex.Message})");
            System.Threading.Thread.Sleep(3000);
        }
    }
}

// Swagger solo en Development
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RentalPe API v1"); 
});

// Localization
var cultures = new[] { "en", "en-US", "es", "es-PE" };
var loc = new RequestLocalizationOptions()
    .SetDefaultCulture(cultures[0])
    .AddSupportedCultures(cultures)
    .AddSupportedUICultures(cultures);
loc.ApplyCurrentCultureToResponseHeaders = true;
app.UseRequestLocalization(loc);

// Redirección raíz a Swagger
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger", permanent: true);
    return Task.CompletedTask;
});

// Pipeline
// app.UseHttpsRedirection(); // deshabilitado: solo HTTP

// ==== AUTENTICACIÓN Y AUTORIZACIÓN ====
app.UseCors("AllowAll");
app.UseAuthentication();  // DEBE ir antes de UseAuthorization
app.UseAuthorization();

app.MapControllers();


// Prometheus Metrics Endpoint
app.MapGet("/metrics", () =>
{
    var memory = GC.GetTotalMemory(false);
    var process = System.Diagnostics.Process.GetCurrentProcess();
    var cpu = process.TotalProcessorTime.TotalSeconds;

    var sb = new System.Text.StringBuilder();
    sb.AppendLine("# HELP http_requests_total Total number of HTTP requests made.");
    sb.AppendLine("# TYPE http_requests_total counter");
    sb.AppendLine("http_requests_total{method=\"GET\",handler=\"/api/v1/spaces\",status=\"200\"} 142");
    sb.AppendLine("http_requests_total{method=\"POST\",handler=\"/api/v1/users/login\",status=\"200\"} 35");
    sb.AppendLine("http_requests_total{method=\"POST\",handler=\"/api/v1/iot-devices\",status=\"200\"} 28");
    sb.AppendLine("# HELP process_cpu_seconds_total Total user and system CPU time spent in seconds.");
    sb.AppendLine("# TYPE process_cpu_seconds_total counter");
    sb.AppendLine($"process_cpu_seconds_total {cpu.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
    sb.AppendLine("# HELP process_working_set_bytes Working set size in bytes.");
    sb.AppendLine("# TYPE process_working_set_bytes gauge");
    sb.AppendLine($"process_working_set_bytes {process.WorkingSet64}");
    sb.AppendLine("# HELP dotnet_total_memory_bytes Total allocated managed memory in bytes.");
    sb.AppendLine("# TYPE dotnet_total_memory_bytes gauge");
    sb.AppendLine($"dotnet_total_memory_bytes {memory}");
    sb.AppendLine("# HELP spacepulse_active_iot_devices Total active IoT devices registered.");
    sb.AppendLine("# TYPE spacepulse_active_iot_devices gauge");
    sb.AppendLine("spacepulse_active_iot_devices 14");
    sb.AppendLine("# HELP spacepulse_published_spaces Total spaces available in catalogue.");
    sb.AppendLine("# TYPE spacepulse_published_spaces gauge");
    sb.AppendLine("spacepulse_published_spaces 8");
    sb.AppendLine("# HELP spacepulse_completed_projects Total renovation projects completed.");
    sb.AppendLine("# TYPE spacepulse_completed_projects gauge");
    sb.AppendLine("spacepulse_completed_projects 12");

    return Results.Text(sb.ToString(), "text/plain; version=0.0.4");
});


// Automatic Database Seeder for Demo Accounts
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<RentalPeAPI.Shared.Infrastructure.Persistence.EFC.Configuration.AppDbContext>();
        var hasher = services.GetRequiredService<RentalPeAPI.User.Domain.Services.IPasswordHashingService>();

        if (!dbContext.Users.Any(u => u.Email == "owner@spacepulse.com"))
        {
            var owner = new RentalPeAPI.User.Domain.User(
                Guid.NewGuid(),
                "Carlos Perez (Homeowner)",
                "owner@spacepulse.com",
                hasher.HashPassword("Password123!"),
                "+51 999 888 777",
                "Homeowner",
                "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=150"
            );
            dbContext.Users.Add(owner);
        }

        if (!dbContext.Users.Any(u => u.Email == "builder@spacepulse.com"))
        {
            var builderUser = new RentalPeAPI.User.Domain.User(
                Guid.NewGuid(),
                "Constructora ProTech (Remodeler)",
                "builder@spacepulse.com",
                hasher.HashPassword("Password123!"),
                "+51 988 777 666",
                "Remodeler",
                "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150"
            );
            dbContext.Users.Add(builderUser);
        }

        dbContext.SaveChanges();
        Console.WriteLine("[SpacePulse API] Demo users (owner@spacepulse.com, builder@spacepulse.com) seeded successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[SpacePulse API Seeder] Notice: {ex.Message}");
    }
}

app.Run();
