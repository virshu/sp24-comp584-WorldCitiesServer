using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorldCitiesModel;

public class Country
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(2)]
    [Unicode(false)]
    public string Iso2 { get; set; } = null!;

    [StringLength(3)]
    [Unicode(false)]
    public string Iso3 { get; set; } = null!;

    [InverseProperty("Country")]
    public virtual ICollection<City> Cities { get; set; } = new List<City>();

}