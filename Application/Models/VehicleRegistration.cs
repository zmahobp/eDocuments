namespace Models;
[Table("VehicleRegistration")]
public class VehicleRegistration : Document
{
    // Name="VehicleRegistration";
    [Required]
    public string VehicleRegistrationNumber { get; set; }

    [MaxLength(20)]
    [Required]
    public string RegistrationNumber { get; set; }

    [Required]
    public DateTime FirstRegistrationDate { get; set; }

    [Required]
    public string LoadCapacity { get; set; }

    [Required]
    public string Weight { get; set; }

    [Required]
    public string SeatNumber { get; set; }

    [Required]
    public string ProductionYear { get; set; }

    [MaxLength(50)]
    [Required]
    public string EngineNumber { get; set; }

    [MaxLength(50)]
    [Required]
    public string ChassisNumber { get; set; }

    [MaxLength(50)]
    [Required]
    public string Brand { get; set; }

    [MaxLength(50)]
    [Required]
    public string Type { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }

    [System.Text.Json.Serialization.JsonIgnore] 
    public RegularUser User { get; set; }
}