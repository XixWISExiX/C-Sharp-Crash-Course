using System.ComponentModel.DataAnnotations;

namespace ContactManager.Contracts.Contacts;

public sealed class UpdateContactRequest // PUT
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 200 characters.")]
    public string Name { get; init; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
    [StringLength(254, ErrorMessage = "Email is too long.")]
    public string? Email { get; init; }

    [StringLength(25, MinimumLength = 7, ErrorMessage = "Phone must be between 7 and 25 characters.")]
    public string? Phone { get; init; }
}
