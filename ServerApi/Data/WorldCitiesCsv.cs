namespace ServerApi.Data;

public class WorldCitiesCsv
{
    public string city { get; set; } = null!;
    public string city_ascii { get; set; } = null!;
    public decimal lat { get; set; }
    public decimal lng { get; set; }
    public string country { get; set; } = null!;
    public string iso2 { get; set; } = null!;
    public string iso3 { get; set; } = null!;
    public decimal? population { get; set; }
    public long id { get; set; }

}