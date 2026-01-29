using Microsoft.EntityFrameworkCore;

namespace Models;
public class DatabaseContext : DbContext
{
    public required DbSet<Document> Documents { get; set; }
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

        modelBuilder.Entity<IDCard>()
            .HasOne(i => i.User)
            .WithOne(u => u.IdCard)
            .HasForeignKey<IDCard>(i => i.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Passport>()
            .HasOne(p => p.User)
            .WithOne(u => u.Passport)
            .HasForeignKey<Passport>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DrivingLicense>()
            .HasOne(d => d.User)
            .WithOne(u => u.DrivingLicense)
            .HasForeignKey<DrivingLicense>(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WeaponPermit>()
            .HasOne(w => w.User)
            .WithOne(u => u.WeaponPermit)
            .HasForeignKey<WeaponPermit>(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<VehicleRegistration>()
            .HasOne(v => v.User)
            .WithOne(u => u.VehicleRegistration)
            .HasForeignKey<VehicleRegistration>(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.User)
            .WithMany(u => u.Appointments)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Station)
            .WithMany()
            .HasForeignKey(a => a.StationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RegularUser>()
            .HasMany(u => u.Appointments)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}