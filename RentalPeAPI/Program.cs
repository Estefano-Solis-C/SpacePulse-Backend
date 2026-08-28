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
        options.UseMySql(cs, ServerVersion.AutoDetect(cs), mySqlOptions =>
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

var app = builder.Build();

// Apply Database Migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
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
app.UseAuthentication();  // DEBE ir antes de UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
