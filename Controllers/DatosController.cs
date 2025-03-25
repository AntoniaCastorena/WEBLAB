using API_WebLabCon_test.Context;
using API_WebLabCon_test.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class DatosController : ControllerBase
{
    private readonly WebLabConaguaContext _context;

    public DatosController(WebLabConaguaContext context)
    {
        _context = context; // Usa guión bajo para campos privados
    }

    /// <summary>
    /// Obtiene datos meteorológicos con filtros opcionales
    /// </summary>
    /// <param name="estacion">ID de la estación</param>
    /// <param name="fromDate">Fecha de inicio (opcional)</param>
    /// <param name="toDate">Fecha de fin (opcional)</param>
    /// <param name="pageNumber">Número de página (comienza en 1)</param>
    /// <param name="pageSize">Tamaño de página</param>
    /// <returns>Lista paginada de datos meteorológicos</returns>
    /// <response code="200">Devuelve los datos encontrados</response>
    /// <response code="404">No se encontraron datos</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Dato>>> Get(
        [FromQuery] string? estacion,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50) // Valor predeterminado más razonable
    {
        var query = _context.Datos
            .Include(d => d.EstacionNavigation) // Incluir datos relacionados
            .AsQueryable();

        // Filtro por estación
        if (!string.IsNullOrEmpty(estacion))
        {
            query = query.Where(d => d.Estacion == estacion);
        }

        // Filtro por rango de fechas
        if (fromDate.HasValue)
        {
            query = query.Where(d => d.Fecha >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(d => d.Fecha <= toDate.Value);
        }

        // Ordenar por fecha descendente
        query = query.OrderByDescending(d => d.Fecha);

        // Contar total de registros para paginación
        var totalItems = await query.CountAsync();

        // Aplicar paginación
        var datos = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        if (datos.Count == 0)
        {
            return NotFound("No se encontraron datos que coincidan con los filtros proporcionados.");
        }

        // Devolver resultado con metadatos de paginación
        var result = new
        {
            TotalItems = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            Items = datos
        };

        return Ok(result);
    }

    /// <summary>
    /// Obtiene un dato meteorológico por su ID
    /// </summary>
    /// <param name="IdDato">ID del dato</param>
    /// <returns>Dato meteorológico</returns>
    /// <response code="200">Devuelve el dato encontrado</response>
    /// <response code="404">No se encontró el dato</response>
    [HttpGet("{IdDato}", Name = "ObtenerDatos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Dato>> Get(int IdDato)
    {
        var dato = await _context.Datos
            .Include(d => d.EstacionNavigation) // Incluir datos relacionados
            .FirstOrDefaultAsync(x => x.IdDato == IdDato);

        if (dato == null)
        {
            return NotFound("Dato no encontrado.");
        }

        return Ok(dato);
    }

    /// <summary>
    /// Obtiene un resumen estadístico de los datos de una estación
    /// </summary>
    /// <param name="estacion">ID de la estación (obligatorio)</param>
    /// <param name="fromDate">Fecha de inicio (opcional)</param>
    /// <param name="toDate">Fecha de fin (opcional)</param>
    /// <returns>Resumen estadístico</returns>
    [HttpGet("resumen")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetResumen(
        [FromQuery] string estacion,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        if (string.IsNullOrEmpty(estacion))
        {
            return BadRequest("El parámetro 'estacion' es obligatorio.");
        }

        var query = _context.Datos.Where(d => d.Estacion == estacion);

        if (fromDate.HasValue)
        {
            query = query.Where(d => d.Fecha >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(d => d.Fecha <= toDate.Value);
        }

        // Verificar si hay datos
        if (!await query.AnyAsync())
        {
            return NotFound("No se encontraron datos para la estación en el período especificado.");
        }

        // Calcular resumen estadístico
        var resumen = await query.GroupBy(d => d.Estacion)
            .Select(g => new
            {
                Estacion = g.Key,
                TotalRegistros = g.Count(),
                FechaInicio = g.Min(d => d.Fecha),
                FechaFin = g.Max(d => d.Fecha),
                TemperaturaPromedio = g.Average(d => d.Temp),
                TemperaturaMaxima = g.Max(d => d.Temp),
                TemperaturaMinima = g.Min(d => d.Temp),
                PrecipitacionTotal = g.Sum(d => d.Prec),
                HumedadRelativaPromedio = g.Average(d => d.Humr)
            })
            .FirstOrDefaultAsync();

        return Ok(resumen);
    }
}