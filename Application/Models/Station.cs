namespace Models;
[Table("Station")]
public class Station
{
    [Key]
    public int ID { get; set; }

    [MaxLength(50)]
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string PhoneNumber { get; set; }

    [MaxLength(50)]
    [Required]
    public string City { get; set; }

    [MaxLength(50)]
    [Required]
    public string Municipality { get; set; }

    [MaxLength(50)]
    [Required]
    public string Street { get; set; }

    [Required]
    public string Number { get; set; }
 
    public List<Appointment> Appointments { get; set; }
}