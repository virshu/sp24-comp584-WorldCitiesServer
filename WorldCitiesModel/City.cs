using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldCitiesModel;

public class City
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")]
    public decimal Lat { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public decimal Lon { get; set; }

    public int CountryId { get; set; }
    public int Population { get; set; }

    [ForeignKey("CountryId")]
    [InverseProperty("Cities")]
    public virtual Country Country { get; set; } = null!;

}