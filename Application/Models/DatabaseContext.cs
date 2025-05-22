using Microsoft.EntityFrameworkCore;

namespace Models;
public class DatabaseContext : DbContext
{
    //public required DbSet<Document> Documents { get; set; }
    public required DbSet<WeaponPermit> WeaponPermits { get; set; }
    public required DbSet<IDCard> IDCards { get; set; }
    public required DbSet<Passport> Passports { get; set; }
    public required DbSet<DrivingLicense> DrivingLicenses { get; set; }
    public required DbSet<VehicleRegistration> VehicleRegistrations { get; set; }
    public required DbSet<Station> Stations { get; set; }
    public required DbSet<Appointment> Appointments { get; set; }
    public required DbSet<UserSupport> UserSupports { get; set; }
    public required DbSet<RegularUser> RegularUsers { get; set; }

    public DatabaseContext(DbContextOptions options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RegularUser>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<RegularUser>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<RegularUser>()
            .HasIndex(u => u.JMBG)
            .IsUnique();

        modelBuilder.Entity<UserSupport>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<UserSupport>()
            .HasIndex(u => u.Username)
            .IsUnique();
    }
}