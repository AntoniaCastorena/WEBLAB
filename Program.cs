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

// 2. Configuración de DbContext con cadena de conexión desde appsettings.json
builder.Services.AddDbContext<WebLabConaguaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WebLabConagua")));

// 3. Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
        policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>())
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// 4. Configuración de Swagger
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

// 5. Configuración del middleware HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebLabConagua API v1");
        c.RoutePrefix = string.Empty; // Swagger UI en la raíz
    });
}

// Middleware personalizado para manejar excepciones
app.UseMiddleware<ExceptionMiddleware>();

// Aplicar CORS antes de `UseAuthorization()`
app.UseCors("AllowWebApp");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
