namespace Models;
[Table("Passport")]
public class Passport : Document
{
    // Name="Passport";
    [Required]
    public string PassportNumber { get; set; }

    [ForeignKey("User")]
    public int UserID { get; set; }
    
    [System.Text.Json.Serialization.JsonIgnore] 
    public RegularUser User { get; set; }
}