using API_WebLabCon_test.Middlewares;
using API_WebLabCon_test.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar los servicios de la aplicación
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );

// 2. Obtener la cadena de conexión desde variables de entorno o appsettings.json
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("WebLabConagua");

builder.Services.AddDbContext<WebLabConaguaContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Configurar CORS dinámico
var allowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(';')
    ?? builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// 4. Configurar Swagger para producción y desarrollo
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WebLabConagua API",
        Version = "v1",
        Description = "API para el sistema de estaciones de CONAGUA",
        Contact = new OpenApiContact
        {
            Name = "Antonia Ortiz",
            Email = "antoniaaoc79@gmail.com"
        }
    });
});

var app = builder.Build(); // Se bloquean modificaciones a `builder.Services`

// 5. Configurar Middleware
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebLabConagua API v1");
        c.RoutePrefix = "swagger"; // Evita acceso directo en producción
    });
}

// Middleware personalizado para manejar excepciones
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AllowWebApp");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
