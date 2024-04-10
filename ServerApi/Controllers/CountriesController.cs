using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApi.DTOs;
using WorldCitiesModel;

namespace ServerApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController(WorldCitiesContext context) : ControllerBase
{
    // GET: api/Countries
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Country>>> GetCountries() => 
        await context.Countries.OrderBy(c => c.Name).ToListAsync();

    [HttpGet("CountryPopulation")]
    public async Task<ActionResult<IEnumerable<CountryPopulation>>> GetCountriesPopulationAsync()
    {
        IQueryable<CountryPopulation> countries = context.Countries.Select(c => new CountryPopulation
        {
            Id = c.Id,
            Name = c.Name,
            Population = c.Cities.Sum(t => t.Population),
            CityCount = c.Cities.Count
        });
        return await countries.ToListAsync();
    }

    [HttpGet("CountryCities/{id:int}")]
    public async Task<ActionResult<IEnumerable<City>>> GetCountryCitiesAsync(int id)
    {
        Country? country = await context.Countries.FindAsync(id);

        if (country == null)
        {
            return NotFound();
        }

        return await context.Cities.Where(t => t.CountryId == id).ToListAsync();
    }

    [HttpGet("Population/{id:int}")]
    [Authorize]
    public async Task<ActionResult<CountryPopulation>> GetCountryPopulationAsync(int id)
    {
        Country? country = await context.Countries.FindAsync(id);
        if (country == null)
        {
            return NotFound();
        }

        return await context.Countries.Where(c => c.Id == id).Select(c => new CountryPopulation
        {
            Id = c.Id,
            Name = c.Name,
            Population = c.Cities.Sum(t => t.Population),
            CityCount = c.Cities.Count
        }).SingleAsync();
    }

    // GET: api/Countries/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Country>> GetCountry(int id)
    {
        Country? country = await context.Countries.FindAsync(id);

        if (country == null)
        {
            return NotFound();
        }

        return country;
    }

    // PUT: api/Countries/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutCountry(int id, Country country)
    {
        if (id != country.Id)
        {
            return BadRequest();
        }

        context.Entry(country).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CountryExists(id))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
    }

    // POST: api/Countries
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Country>> PostCountry(Country country)
    {
        context.Countries.Add(country);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetCountry", new { id = country.Id }, country);
    }

    // DELETE: api/Countries/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        Country? country = await context.Countries.FindAsync(id);
        if (country == null)
        {
            return NotFound();
        }

        context.Countries.Remove(country);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool CountryExists(int id) => context.Countries.Any(e => e.Id == id);
}