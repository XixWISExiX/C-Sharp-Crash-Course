using ContactManager.Contracts.Contacts;
using ContactManager.Data.Entities;

namespace ContactManager.Api.Adapters;

public static class ContactAdapter
{
    public static ContactDto ToDto(ContactEntity e) =>
        new(
            e.Id,
            e.Name,
            e.Email,
            e.Phone
        );

    public static ContactEntity ToEntity(CreateContactRequest req) =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = req.Name.Trim(),
            Email = string.IsNullOrWhiteSpace(req.Email) ? null : req.Email.Trim(),
            Phone = string.IsNullOrWhiteSpace(req.Phone) ? null : req.Phone.Trim(),
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

    public static void Apply(UpdateContactRequest req, ContactEntity e)
    {
        e.Name = req.Name.Trim();
        e.Email = string.IsNullOrWhiteSpace(req.Email) ? null : req.Email.Trim();
        e.Phone = string.IsNullOrWhiteSpace(req.Phone) ? null : req.Phone.Trim();
        e.UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}
