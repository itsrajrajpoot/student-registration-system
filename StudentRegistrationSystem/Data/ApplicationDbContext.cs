using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentRegistrationSystem.Models;

namespace StudentRegistrationSystem.Data;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser>

{
    public DbSet<Student> Students { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Student>()
            .HasIndex(s => s.Email)
            .IsUnique();
        builder.Entity<Student>()
            .HasIndex(s => s.RollNumber)
            .IsUnique();

        builder.Entity<Student>()
            .HasQueryFilter(s => !s.IsDeleted);
    }
}