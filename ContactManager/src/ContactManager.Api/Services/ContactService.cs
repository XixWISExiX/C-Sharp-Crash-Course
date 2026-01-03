using ContactManager.Api.Adapters;
using ContactManager.Contracts.Contacts;
using ContactManager.Data;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Api.Services;

public sealed class ContactService
{
    private readonly ContactManagerDbContext _db;

    public ContactService(ContactManagerDbContext db) => _db = db;

    public async Task<List<ContactDto>> GetAllAsync(CancellationToken ct) =>
        await _db.Contacts.AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => ContactAdapter.ToDto(c))
            .ToListAsync(ct);

    public async Task<ContactDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var e = await _db.Contacts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return e is null ? null : ContactAdapter.ToDto(e);
    }

    public async Task<ContactDto> CreateAsync(CreateContactRequest req, CancellationToken ct)
    {
        var e = ContactAdapter.ToEntity(req);
        _db.Contacts.Add(e);
        await _db.SaveChangesAsync(ct);
        return ContactAdapter.ToDto(e);
    }

    public async Task<ContactDto?> UpdateAsync(Guid id, UpdateContactRequest req, CancellationToken ct)
    {
        var e = await _db.Contacts.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return null;

        ContactAdapter.Apply(req, e);
        await _db.SaveChangesAsync(ct);
        return ContactAdapter.ToDto(e);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var e = await _db.Contacts.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return false;

        _db.Contacts.Remove(e);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
