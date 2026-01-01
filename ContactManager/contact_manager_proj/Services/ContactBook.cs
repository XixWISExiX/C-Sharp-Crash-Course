using System;
using System.Collections.Generic;
using System.Linq;

namespace Contacting
{
    public class ContactBook
    {
        private readonly Dictionary<string, Contact> _contacts = new(StringComparer.OrdinalIgnoreCase);

        public bool Add(Contact c)
        {
            // returns false if already exists
            return _contacts.TryAdd(c.name, c);
        }

        public bool Upsert(Contact c)
        {
            _contacts[c.name] = c;
            return true;
        }

        public bool TryGet(string name, out Contact contact) => _contacts.TryGetValue(name, out contact!);

        public bool Delete(string name) => _contacts.Remove(name);

        public IEnumerable<Contact> ListAll() => _contacts.Values;

        public IEnumerable<Contact> SearchByName(string query) => _contacts.Values.Where(c => c.name.Contains(query, StringComparison.OrdinalIgnoreCase)).OrderBy(c => c.name); // LINQ

        public IEnumerable<Contact> SortedByName() => _contacts.Values.OrderBy(c => c.name); // LINQ
    }
}
