using Microsoft.EntityFrameworkCore;
using SafeCityAPI.Models;

namespace SafeCityAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Report> Reports { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Używamy małych liter (snake_case) - standard PostgreSQL
        
        // Report -> reports
        modelBuilder.Entity<Report>(entity =>
        {
            entity.ToTable("reports");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ReportedAt).HasColumnName("reported_at");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
            entity.Property(e => e.Category)
                .HasColumnName("category")
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(ReportCategory.Other);
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.IpAddress).HasColumnName("ip_address");
            
            entity.HasIndex(e => e.ReportedAt);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IpAddress);
            entity.HasIndex(e => e.Category);
        });

        // User -> users
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Username).HasColumnName("username");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.IsBanned).HasColumnName("is_banned");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}