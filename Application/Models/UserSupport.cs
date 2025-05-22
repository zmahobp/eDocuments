namespace Models;
[Table("UserSupport")]
public class UserSupport
{
    [Key]
    public int ID { get; set; }

    [MaxLength(50)]
    [Required]
    public string FirstName { get; set; }

    [MaxLength(50)]
    [Required]
    public string LastName { get; set; }

    [MaxLength(50)]
    [Required]
    public string Username { get; set; }

    [NotMapped]
    public string Password { get; set; }

    [MaxLength(100)]
    [Required]
    public string HashPassword { get; set; }

    [MaxLength(50)]
    [Required]
    public string Email { get; set; }

    public string SecretKey { get; set; }

    public bool Support { get; set; }

    public bool Authenticate(string enteredPassword)
    {
        return BCrypt.Net.BCrypt.Verify(enteredPassword, HashPassword);
    }

    public void SetHash()
    {
        this.HashPassword = BCrypt.Net.BCrypt.HashPassword(this.Password);
    }

    public void SetSupport()
    {
        Support = true;
    }

    /// <summary>
    /// Generates a secret key of the desired length using a cryptographic random number generator.
    /// </summary>
    public void GenerateSecretKey()
    {
        // Defines the length of the secret key you want to generate
        int keyLength = 32;

        // Creates a byte array with enough space for the generated secret key
        byte[] keyBytes = new byte[keyLength];

        // Uses a cryptographic random number generator to generate the secret key
        using (var generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(keyBytes);
        }

        // Converts the key bytes to a string using Base64 encoding
        string secretKey = Convert.ToBase64String(keyBytes);

        // Saves the generated secret key in the SecretKey property
        SecretKey = secretKey;
    }
}