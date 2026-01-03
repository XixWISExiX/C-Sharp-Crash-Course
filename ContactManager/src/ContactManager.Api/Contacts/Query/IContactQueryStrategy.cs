using ContactManager.Contracts.Contacts;

namespace ContactManager.Api.Contacts.Query;

public interface IContactQueryStrategy
{
    string Name { get; }
    IEnumerable<ContactDto> Apply(IEnumerable<ContactDto> input, string query);
}
