using ContactManager.Contracts.Contacts;
using Microsoft.Extensions.Caching.Memory;

namespace ContactManager.Api.Contacts;

public sealed class CachedContactsClient : IContactsClient
{
    private readonly IContactsClient _inner;
    private readonly IMemoryCache _cache;

    public CachedContactsClient(IContactsClient inner, IMemoryCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<ContactDto?> GetAsync(Guid id, CancellationToken ct)
    {
        var key = $"contact:{id}";
        if (_cache.TryGetValue<ContactDto>(key, out var cached)) return cached;

        var dto = await _inner.GetAsync(id, ct);
        if (dto is not null)
            _cache.Set(key, dto, TimeSpan.FromMinutes(2));

        return dto;
    }

    public Task<List<ContactDto>> ListAsync(CancellationToken ct) => _inner.ListAsync(ct);

    public async Task<ContactDto> UpsertAsync(Guid? id, string name, string? email, string? phone, CancellationToken ct)
    {
        var dto = await _inner.UpsertAsync(id, name, email, phone, ct);
        _cache.Remove($"contact:{dto.Id}");
        return dto;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        await _inner.DeleteAsync(id, ct);
        _cache.Remove($"contact:{id}");
    }
}
