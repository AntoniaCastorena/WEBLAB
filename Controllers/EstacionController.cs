using API_WebLabCon_test.Context;
using API_WebLabCon_test.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_WebLabCon_test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstacionController : ControllerBase
    {
        private readonly WebLabConaguaContext context;

        public EstacionController(WebLabConaguaContext context)
        {
            this.context = context;
        }

        // GET: api/Estaciones
        [HttpGet]
        public ActionResult<IEnumerable<Estacione>> Get(
            [FromQuery] string? nombreEstacion,
            [FromQuery] string? municipio,
            [FromQuery] string? estado,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = context.Estaciones
                .Include(e => e.MunicipioNavigation)
                    .ThenInclude(m => m.EstadoNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(nombreEstacion))
            {
                query = query.Where(e => e.NombreEstacion.Contains(nombreEstacion));
            }

            if (!string.IsNullOrEmpty(municipio))
            {
                query = query.Where(e => e.Municipio == municipio);
            }

            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(e => e.MunicipioNavigation.Estado == estado);
            }

            var totalItems = query.Count();

            var estaciones = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (estaciones.Count == 0)
            {
                return NotFound("No se encontraron estaciones que coincidan con los criterios de búsqueda.");
            }

            var result = new
            {
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                Items = estaciones
            };

            return Ok(result);
        }

        // GET api/Estaciones/id/{IdEstacion}
        [HttpGet("id/{IdEstacion}")]
        public ActionResult<Estacione> GetById(string IdEstacion)
        {
            var estacion = context.Estaciones.FirstOrDefault(x => x.IdEstacion == IdEstacion);

            if (estacion == null)
            {
                return NotFound("Estación no encontrada.");
            }

            return Ok(estacion);
        }

        // GET api/Estaciones/nombre/{NombreEstacion}
        [HttpGet("nombre/{NombreEstacion}")]
        public ActionResult<Estacione> GetByName(string NombreEstacion)
        {
            var estaciones = context.Estaciones
                .Where(x => x.NombreEstacion.Contains(NombreEstacion))
                .ToList();

            if (estaciones.Count == 0)
            {
                return NotFound("No se encontraron estaciones con ese nombre.");
            }

            return Ok(estaciones);
        }
    }
}
