using ContactManager.Contracts.Contacts;

namespace ContactManager.Api.Contacts;

public interface IContactsClient
{
    Task<ContactDto?> GetAsync(Guid id, CancellationToken ct);
    Task<ContactDto> UpsertAsync(Guid? id, string name, string? email, string? phone, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
    Task<List<ContactDto>> ListAsync(CancellationToken ct);
}
