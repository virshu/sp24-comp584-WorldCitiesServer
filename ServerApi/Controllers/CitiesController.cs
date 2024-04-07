using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApi.DTOs;
using WorldCitiesModel;

namespace ServerApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CitiesController(WorldCitiesContext context) : ControllerBase
{
    // GET: api/Cities
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CityDto>>> GetCities()
    {
        IQueryable<CityDto> cityQry = context.Cities.Select(t => new CityDto
        {
            Id = t.Id,
            Name = t.Name,
            Lat = t.Lat,
            Lon = t.Lon,
            Country = t.Country.Name
        }).Take(100);
        return await cityQry.ToListAsync();
    }

    // GET: api/Cities/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<City>> GetCity(int id)
    {
        City? city = await context.Cities.FindAsync(id);

        if (city == null)
        {
            return NotFound();
        }

        return city;
    }

    // PUT: api/Cities/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutCity(int id, City city)
    {
        if (id != city.Id)
        {
            return BadRequest();
        }

        context.Entry(city).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CityExists(id))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
    }

    // POST: api/Cities
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<City>> PostCity(City city)
    {
        context.Cities.Add(city);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetCity", new { id = city.Id }, city);
    }

    // DELETE: api/Cities/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCity(int id)
    {
        City? city = await context.Cities.FindAsync(id);
        if (city == null)
        {
            return NotFound();
        }

        context.Cities.Remove(city);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool CityExists(int id) => context.Cities.Any(e => e.Id == id);
}