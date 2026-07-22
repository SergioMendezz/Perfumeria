using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Interfaces.Servicios;
using DA;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

const string PoliticaCorsSpas = "PoliticaCorsSpas";

builder.Services.AddControllers();

builder.Services.AddScoped<IPerfumeDA, DA.PerfumeDA>();
builder.Services.AddScoped<IPerfumeFlujo, Flujo.PerfumeFlujo>();
builder.Services.AddScoped<IUsuarioDA, DA.UsuarioDA>();
builder.Services.AddScoped<IJwtService, Servicios.JwtService>();
builder.Services.AddScoped<IAuthFlujo, Flujo.AuthFlujo>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo { Title = "Perfumeria.Api", Version = "v1" });
    opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
         Description = "Ingresar únicamente el token JWT, sin el prefijo 'Bearer ' — Swagger lo agrega automáticamente"
    });
    opciones.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", null),
            new List<string>()
        }
    });
});

builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy(PoliticaCorsSpas, politica =>
    {
        politica.WithOrigins(
                builder.Configuration.GetSection("OrigenesPermitidos").Get<string[]>()
                    ?? ["http://localhost:5173", "http://localhost:5174"])
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("No se encontró 'Jwt:Key' en la configuración.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones =>
    {
        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IRepositorioDapper, RepositorioDapper>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(PoliticaCorsSpas);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
