namespace Contacting
{
    public record Contact(string name, string phone, string? Email = null);

    public static class ContactValidator
    {
        public static void Validate(Contact c)
        {
            if (string.IsNullOrWhiteSpace(c.name)) throw new ArgumentException("Name cannot be empty.");
            if (string.IsNullOrWhiteSpace(c.phone)) throw new ArgumentException("Phone cannot be empty.");
            foreach (char ch in c.phone) if (!char.IsDigit(ch)) throw new ArgumentException("Phone must contain digits only.");
        }
    }
}

