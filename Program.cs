using API_WebLabCon_test.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de controladores
builder.Services.AddControllers();

// 2. Configuración de DbContext para Entity Framework
builder.Services.AddDbContext<WebLabConaguaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WebLabConagua")));

// 3. Configuración de JSON para evitar errores de referencia circular (NewtonsoftJson)
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );

// 4. Configuración de Swagger/OpenAPI para desarrollo
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WebLabConagua API",
        Version = "v1",
        Description = "API para el sistema de estaciones meteorológicas",
        Contact = new OpenApiContact
        {
            Name = "Tu Nombre",
            Email = "tu.email@ejemplo.com"
        }
    });

    // Habilitar comentarios XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// ✅ **Configuración de CORS**
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", builder =>
    {
        builder.WithOrigins("http://localhost:3000") // URL de tu aplicación Next.js
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials(); // Si necesitas enviar cookies
    });

    // Política alternativa para desarrollo
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build(); // Aquí se bloquean las modificaciones a `builder.Services`

// Configurar la canalización de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebLabConagua API v1");
        c.RoutePrefix = string.Empty; // Para que Swagger UI sea la página de inicio
    });
}

// Program.cs
// Registrar el middleware
app.UseMiddleware<ExceptionMiddleware>();

// Aplicar CORS antes de `UseAuthorization()`
app.UseCors("AllowWebApp"); // O "AllowAll" en desarrollo

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
