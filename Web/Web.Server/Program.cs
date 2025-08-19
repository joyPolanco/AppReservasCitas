using Aplicacion.Casosdeuso;
using Aplicacion.Interfaces;
using Aplicacion.Servicios;
using Dominio.Entidades;
using Dominio.Entidades.Models;
using Dominio.Interfaces;
using Infraestructura.Persistencia;
using Infraestructura.Persistencia.Contexto;
using Infraestructura.Persistencia.Loggers;
using Infraestructura.Persistencia.Repositorios;
using Infraestructura.ServiciosExternos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddDbContext<SistemaReservasContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<JwtConfiguration>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPasswordManager, PasswordEncryptor>();
builder.Services.AddScoped(typeof(IRepositorio<>), typeof(RepoGenerico<>));
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepositorio>();
builder.Services.AddScoped<IConfiguracionService, ConfiguracionService>();
builder.Services.AddScoped<IPlanificacionRepositorio, PlanificacionRepositorio>();
builder.Services.AddScoped<IPlanificacionService, PlanificacionService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISlotRepositorio, SlotRepositorio>();
builder.Services.AddScoped<IEstacionRepositorio, EstacionRepo>();
builder.Services.AddScoped<IReservaRepositorio, ReservaRepo>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IEmailService, SmtpEmailAdapter>();
builder.Services.AddScoped<ITokenRepositorio, TokenRepositorio>();
builder.Services.AddHostedService<ReservaCleanupService>();
builder.Services.AddScoped<IDiaHabilitadoRepo, DiaHabilitadoRepo>();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

builder.Services.AddOpenApi();

// JWT Authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(JwtConfiguration.Secret)),

            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = "role"
        };
    });
IRepositorio<Log> repoLogs = new RepoGenerico<Log>(new SistemaReservasContext());

var logger = LoggerContext.GetLogger(repoLogs);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:50983") 
              .AllowAnyHeader()
              .AllowAnyMethod()

            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("PermitirFrontend");

app.UseDefaultFiles();
app.MapStaticAssets();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
