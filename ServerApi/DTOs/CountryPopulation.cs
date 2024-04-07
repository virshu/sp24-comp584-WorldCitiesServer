namespace ServerApi.DTOs;

public class CountryPopulation
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Population { get; set; }
    public int CityCount { get; set; }
}
