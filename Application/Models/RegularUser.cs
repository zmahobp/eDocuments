namespace Models;
[Table("RegularUser")]
public class RegularUser
{
    [Key]
    public int ID { get; set; }

    [MaxLength(50)]
    [Required]
    public string FirstName { get; set; }

    [MaxLength(50)]
    [Required]
    public string ParentName { get; set; }

    [MaxLength(13)]
    [Required]
    public string JMBG { get; set; }

    [MaxLength(50)]
    [Required]
    public string LastName { get; set; }

    [MaxLength(50)]
    [Required]
    public string Username { get; set; }

    [NotMapped]
    public string Password { get; set; }

    [MaxLength(100)]
    public string HashPassword { get; set; }
    
    [MaxLength(50)]
    [Required]
    public string Email { get; set; }

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

    [MaxLength(50)]
    [Required]
    public string Phone { get; set; }

    [Required]
    public DateTime BirthDate { get; set; }

    [MaxLength(50)]
    [Required]
    public string BirthPlace { get; set; }

    [Required]
    public char Gender { get; set; }
    
    public string Photo { get; set; }

    public bool OfficialPerson { get; set; }

    public string SecretKey { get; set; }

    // 1:1 RELATIONSHIPS
    public IDCard IDCard { get; set; }
    public Passport Passport { get; set; }    
    public DrivingLicense DrivingLicense { get; set; }
    public VehicleRegistration VehicleRegistration { get; set; }
    public WeaponPermit WeaponPermit { get; set; }

    // 1:N RELATIONSHIP
    public List<Appointment> Appointments { get; set; }

    public bool Authenticate(string enteredPassword)
    {
        return BCrypt.Net.BCrypt.Verify(enteredPassword, HashPassword);
    }

    public void SetHash(string enteredPassword)
    {
        this.Password = enteredPassword;
        this.HashPassword = BCrypt.Net.BCrypt.HashPassword(this.Password);
    }

    public void GenerateSecretKey()
    {
        // Define the length of the secret key you want to generate
        int keyLength = 32;

        // Create a byte array with enough space for the generated secret key
        byte[] keyBytes = new byte[keyLength];

        // Use a cryptographic random number generator to generate the secret key
        using (var generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(keyBytes);
        }

        // Convert the key bytes to a string using Base64 encoding
        string secretKey = Convert.ToBase64String(keyBytes);

        // Save the generated secret key in the SecretKey property
        SecretKey = secretKey;
    }

    public void SetOfficialPerson()
    {
        this.OfficialPerson = true;
    }

    public void RemoveOfficialPerson()
    {
        this.OfficialPerson = false;
    }

    public bool CheckOldPassword(string enteredOldPassword, RegularUser user)
    {
        return user.Authenticate(enteredOldPassword);
    }
}