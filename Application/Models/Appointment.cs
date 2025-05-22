namespace Models;
[Table("Appointment")]
public class Appointment
{
    [Key]
    public int ID { get; set; }

    [Required]
    public DateTime DateTime { get; set; }

    [Required]
    [System.Text.Json.Serialization.JsonIgnore] 
    public Station Station { get; set; }
     
    [System.Text.Json.Serialization.JsonIgnore] 
    public RegularUser User { get; set; }

    [MaxLength(200)]
    public string Description { get; set; }

    [MaxLength(20)]
    [Required]
    public string Status { get; set; }
}