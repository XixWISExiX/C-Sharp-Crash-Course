using System.ComponentModel.DataAnnotations;

namespace ContactManager.Data.Entities;

public sealed class ContactEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(254)]
    public string? Email { get; set; }

    [MaxLength(25)]
    public string? Phone { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
