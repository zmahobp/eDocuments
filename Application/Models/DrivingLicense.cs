namespace Models;
[Table("DrivingLicense")]
public class DrivingLicense : Document
{
    // Name = "DrivingLicense";

    [Required]
    public string DrivingLicenseNumber { get; set; }

    [Required]
    public string VehicleCategories { get; set; }

    [ForeignKey("User")]
    public int UserID { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public RegularUser User { get; set; }
}