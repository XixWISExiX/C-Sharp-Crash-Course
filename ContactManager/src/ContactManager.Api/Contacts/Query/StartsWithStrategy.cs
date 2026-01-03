using ContactManager.Contracts.Contacts;

namespace ContactManager.Api.Contacts.Query;

public sealed class StartsWithStrategy : IContactQueryStrategy
{
    public string Name => "startsWith";

    public IEnumerable<ContactDto> Apply(IEnumerable<ContactDto> input, string query) =>
        input.Where(c => c.Name.StartsWith(query, StringComparison.OrdinalIgnoreCase));
}
