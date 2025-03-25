using API_WebLabCon_test.Context;
using API_WebLabCon_test.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class EstadoController : ControllerBase
{
    private readonly WebLabConaguaContext _context;

    public EstadoController(WebLabConaguaContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Estado>>> GetAll()
    {
        var estados = await _context.Estados.ToListAsync();
        return Ok(estados);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Estado>> GetById(string id)
    {
        var estado = await _context.Estados.FindAsync(id);

        if (estado == null)
        {
            return NotFound("Estado no encontrado.");
        }

        return Ok(estado);
    }
}