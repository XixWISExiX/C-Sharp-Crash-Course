using ContactManager.Contracts.Contacts;
using ContactManager.Grpc.Contacts;
using Grpc.Core;

namespace ContactManager.Api.Contacts;

public sealed class GrpcContactsClient : IContactsClient
{
    private readonly ContactsService.ContactsServiceClient _grpc;
    public GrpcContactsClient(ContactsService.ContactsServiceClient grpc) => _grpc = grpc;

    public async Task<ContactDto?> GetAsync(Guid id, CancellationToken ct)
    {
        try
        {
            var r = await _grpc.GetContactAsync(new GetContactRequest { Id = id.ToString() }, cancellationToken: ct);
            return new ContactDto(Guid.Parse(r.Id), r.Name,
                string.IsNullOrWhiteSpace(r.Email) ? null : r.Email,
                string.IsNullOrWhiteSpace(r.Phone) ? null : r.Phone);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<List<ContactDto>> ListAsync(CancellationToken ct)
    {
        var r = await _grpc.ListContactsAsync(new ListContactsRequest(), cancellationToken: ct);
        return r.Contacts.Select(x => new ContactDto(Guid.Parse(x.Id), x.Name,
            string.IsNullOrWhiteSpace(x.Email) ? null : x.Email,
            string.IsNullOrWhiteSpace(x.Phone) ? null : x.Phone)).ToList();
    }

    public async Task<ContactDto> UpsertAsync(Guid? id, string name, string? email, string? phone, CancellationToken ct)
    {
        var r = await _grpc.UpsertContactAsync(new UpsertContactRequest
        {
            Id = id?.ToString() ?? "",
            Name = name,
            Email = email ?? "",
            Phone = phone ?? ""
        }, cancellationToken: ct);

        return new ContactDto(Guid.Parse(r.Id), r.Name,
            string.IsNullOrWhiteSpace(r.Email) ? null : r.Email,
            string.IsNullOrWhiteSpace(r.Phone) ? null : r.Phone);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        await _grpc.DeleteContactAsync(new DeleteContactRequest { Id = id.ToString() }, cancellationToken: ct);
    }
}
