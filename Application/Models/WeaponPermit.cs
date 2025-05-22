namespace Models;
[Table("WeaponPermit")]
public class WeaponPermit : Document
{
    // Name = "WeaponPermit";
    [Required]
    public string WeaponPermitNumber { get; set; }

    [MaxLength(200)]
    [Required]
    public string WeaponTypes { get; set; }

    [Required]
    public string WeaponQuantity { get; set; }

    [Required]
    public string WeaponTypeCount { get; set; }

    [Required]
    public string WeaponCaliber { get; set; }

    [MaxLength(200)]
    [Required]
    public string UsageLocation { get; set; }

    [MaxLength(200)]
    [Required]
    public string UsagePurpose { get; set; }

    [ForeignKey("User")]
    public int UserID { get; set; }

    [System.Text.Json.Serialization.JsonIgnore] 
    public RegularUser User { get; set; }
}