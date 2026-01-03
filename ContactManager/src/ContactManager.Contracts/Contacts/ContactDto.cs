namespace ContactManager.Contracts.Contacts;

public sealed record ContactDto(
    Guid Id,
    string Name,
    string? Email,
    string? Phone
);
