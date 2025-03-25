using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_WebLabCon_test.Models;
using API_WebLabCon_test.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class MunicipioController : ControllerBase
{
    private readonly WebLabConaguaContext _context;

    public MunicipioController(WebLabConaguaContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Municipio>>> Get([FromQuery] string? estado)
    {
        var query = _context.Municipios.AsQueryable();

        if (!string.IsNullOrEmpty(estado))
        {
            query = query.Where(m => m.Estado == estado);
        }

        var municipios = await query.ToListAsync();

        if (!municipios.Any())
        {
            return NotFound("No se encontraron municipios.");
        }

        return Ok(municipios);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Municipio>> GetById(string id)
    {
        var municipio = await _context.Municipios
            .Include(m => m.EstadoNavigation)
            .FirstOrDefaultAsync(m => m.IdMunicipio == id);

        if (municipio == null)
        {
            return NotFound("Municipio no encontrado.");
        }

        return Ok(municipio);
    }
}