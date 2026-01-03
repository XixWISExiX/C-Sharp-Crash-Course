using ContactManager.Contracts.Contacts;

namespace ContactManager.Api.Contacts.Query;

public sealed class ContainsStrategy : IContactQueryStrategy
{
    public string Name => "contains";

    public IEnumerable<ContactDto> Apply(IEnumerable<ContactDto> input, string query) =>
        input.Where(c => c.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
}
