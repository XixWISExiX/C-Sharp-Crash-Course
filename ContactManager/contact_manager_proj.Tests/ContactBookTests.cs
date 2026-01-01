using Contacting;
namespace contact_manager_proj.Tests;
public class ContactBookTests
{
    [Fact]
    public void Add_ShouldNotAllowDuplicateNames()
    {
        var book = new ContactBook();
        book.Add(new Contact("A", "123"));
        bool addedAgain = book.Add(new Contact("A", "999"));
        Assert.False(addedAgain);
    }
}
