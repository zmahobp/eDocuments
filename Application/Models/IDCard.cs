namespace Models;
[Table("IDCard")]
public class IDCard : Document
{
    // Name="IDCard";
    [Required]
    public string IDCardNumber { get; set; }

    [ForeignKey("User")]
    public int UserID { get; set; }

    [System.Text.Json.Serialization.JsonIgnore] 
    public RegularUser User { get; set; }
}