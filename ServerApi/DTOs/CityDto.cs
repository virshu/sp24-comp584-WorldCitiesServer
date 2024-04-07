namespace ServerApi.DTOs;

public class CityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Lat { get; set; }
    public decimal Lon { get; set; }
    public string Country { get; set; } = null!;
}