using ContactManager.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Data;

public sealed class ContactManagerDbContext : DbContext
{
    public ContactManagerDbContext(DbContextOptions<ContactManagerDbContext> options)
        : base(options) { }

    public DbSet<ContactEntity> Contacts => Set<ContactEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var contacts = modelBuilder.Entity<ContactEntity>();

        contacts.HasKey(c => c.Id);

        contacts.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        contacts.Property(c => c.Email)
            .HasMaxLength(254);

        contacts.Property(c => c.Phone)
            .HasMaxLength(25);

        contacts.Property(c => c.CreatedAtUtc).IsRequired();
        contacts.Property(c => c.UpdatedAtUtc).IsRequired();

        // Optional: simple index for lookup
        contacts.HasIndex(c => c.Email);
    }
}
