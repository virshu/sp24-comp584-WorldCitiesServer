﻿using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApi.Data;
using WorldCitiesModel;

namespace ServerApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SeedController(WorldCitiesContext db, IHostEnvironment environment, 
    UserManager<WorldCitiesUser> userManager) : ControllerBase
{
    private readonly string _pathName = Path.Combine(environment.ContentRootPath, "Data/worldcities.csv");

    [HttpPost("Countries")]
    public async Task<IActionResult> ImportCountriesAsync()
    {
        // create a lookup dictionary containing all the countries already existing 
        // into the Database (it will be empty on first run).
        Dictionary<string, Country> countriesByName = await db.Countries
            .AsNoTracking().ToDictionaryAsync(x => x.Name, StringComparer.OrdinalIgnoreCase);

        CsvConfiguration config = new(CultureInfo.InvariantCulture) {
            HasHeaderRecord = true,
            HeaderValidated = null
        };

        using StreamReader reader = new(_pathName);
        using CsvReader csv = new(reader, config);

        List<WorldCitiesCsv> records = csv.GetRecords<WorldCitiesCsv>().ToList();
        foreach (WorldCitiesCsv record in records) {
            if (countriesByName.ContainsKey(record.country)) {
                continue;
            }

            Country country = new() {
                Name = record.country,
                Iso2 = record.iso2,
                Iso3 = record.iso3
            };
            await db.Countries.AddAsync(country);
            countriesByName.Add(record.country, country);
        }

        await db.SaveChangesAsync();

        return new JsonResult(countriesByName.Count);

    }

    [HttpPost("Cities")]
    public async Task<IActionResult> ImportCitiesAsync() {
        Dictionary<string, Country> countries = await db.Countries//.AsNoTracking()
            .ToDictionaryAsync(c => c.Name);

        CsvConfiguration config = new(CultureInfo.InvariantCulture) {
            HasHeaderRecord = true,
            HeaderValidated = null
        };
        int cityCount = 0;
        using (StreamReader reader = new(_pathName))
        using (CsvReader csv = new(reader, config)) {
            IEnumerable<WorldCitiesCsv> records = csv.GetRecords<WorldCitiesCsv>();
            foreach (WorldCitiesCsv record in records) {
                if (!countries.TryGetValue(record.country, out Country? value)) {
                    Console.WriteLine($"Not found country for {record.city}");
                    return NotFound(record);
                }

                if (!record.population.HasValue || string.IsNullOrEmpty(record.city_ascii)) {
                    Console.WriteLine($"Skipping {record.city}");
                    continue;
                }
                City city = new() {
                    Name = record.city,
                    Lat = record.lat,
                    Lon = record.lng,
                    Population = (int)record.population.Value,
                    CountryId = value.Id
                };
                await db.Cities.AddAsync(city);
                cityCount++;
            }
            await db.SaveChangesAsync();
        }
        return new JsonResult(cityCount);
    }

    [HttpPost("Users")]
    public async Task<IActionResult> ImportUsersAsync()
    {
        (string name, string email) = ("user1", "comp584@csun.edu");
        WorldCitiesUser user = new() {
            UserName = name,
            Email = email,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        if (await userManager.FindByNameAsync(name) is not null)
        {
            user.UserName = "user2";
        }
        _ = await userManager.CreateAsync(user, "P@ssw0rd!")
            ?? throw new InvalidOperationException();
        user.EmailConfirmed = true;
        user.LockoutEnabled = false;
        await db.SaveChangesAsync();

        return Ok();
    }

}