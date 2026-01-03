using System.Globalization;
using ContactManager.Data;
using ContactManager.Data.Entities;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ContactManager.Grpc.Contacts;

namespace ContactManager.Grpc.Services;

public sealed class ContactsGrpcService : ContactsService.ContactsServiceBase
{
    private readonly ContactManagerDbContext _db;

    public ContactsGrpcService(ContactManagerDbContext db) => _db = db;

    public override async Task<ContactReply> GetContact(GetContactRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid GUID id."));

        // Optional: simulate work to demo deadline behavior later
        // await Task.Delay(500, context.CancellationToken);

        var entity = await _db.Contacts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, context.CancellationToken);
        if (entity is null)
            throw new RpcException(new Status(StatusCode.NotFound, "Contact not found."));

        return ToReply(entity);
    }

    public override async Task<ContactReply> UpsertContact(UpsertContactRequest request, ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Name is required."));

        ContactEntity entity;

        // Create if id empty, else update
        if (string.IsNullOrWhiteSpace(request.Id))
        {
            entity = new ContactEntity
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
                CreatedAtUtc = DateTimeOffset.UtcNow,
                UpdatedAtUtc = DateTimeOffset.UtcNow
            };

            _db.Contacts.Add(entity);
        }
        else
        {
            if (!Guid.TryParse(request.Id, out var id))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid GUID id."));

            entity = await _db.Contacts.FirstOrDefaultAsync(c => c.Id == id, context.CancellationToken);
            if (entity is null)
            {
                // Upsert choice: create new with provided id
                entity = new ContactEntity
                {
                    Id = id,
                    CreatedAtUtc = DateTimeOffset.UtcNow
                };
                _db.Contacts.Add(entity);
            }

            entity.Name = request.Name.Trim();
            entity.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
            entity.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
            entity.UpdatedAtUtc = DateTimeOffset.UtcNow;
        }

        await _db.SaveChangesAsync(context.CancellationToken);
        return ToReply(entity);
    }

    private static ContactReply ToReply(ContactEntity e) =>
        new()
        {
            Id = e.Id.ToString("D", CultureInfo.InvariantCulture),
            Name = e.Name ?? "",
            Email = e.Email ?? "",
            Phone = e.Phone ?? ""
        };
    public override async Task<ListContactsReply> ListContacts(ListContactsRequest request, ServerCallContext context)
    {
        var items = await _db.Contacts.AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(context.CancellationToken);

        var reply = new ListContactsReply();
        reply.Contacts.AddRange(items.Select(e => new ContactReply
        {
            Id = e.Id.ToString(),
            Name = e.Name ?? "",
            Email = e.Email ?? "",
            Phone = e.Phone ?? ""
        }));

        return reply;
    }

    public override async Task<DeleteContactReply> DeleteContact(DeleteContactRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid GUID id."));

        var entity = await _db.Contacts.FirstOrDefaultAsync(c => c.Id == id, context.CancellationToken);
        if (entity is null)
        {
            // choose behavior: either return deleted=false or throw NotFound
            // I recommend NotFound for consistency with REST.
            throw new RpcException(new Status(StatusCode.NotFound, "Contact not found."));
        }

        _db.Contacts.Remove(entity);
        await _db.SaveChangesAsync(context.CancellationToken);

        return new DeleteContactReply { Deleted = true };
    }

}
