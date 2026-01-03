using System.ComponentModel.DataAnnotations;
using ContactManager.Contracts.Contacts;
//using Xunit;

namespace ContactManager.Tests.Unit;

public sealed class ValidationTests
{
    [Fact]
    public void CreateContactRequest_MissingName_FailsValidation()
    {
        var req = new CreateContactRequest
        {
            Name = "",                // invalid
            Email = "josh@example.com",
            Phone = "1234567890"
        };

        var results = Validate(req);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateContactRequest.Name)));
    }

    [Fact]
    public void CreateContactRequest_BadEmail_FailsValidation()
    {
        var req = new CreateContactRequest
        {
            Name = "Joshua",
            Email = "not-an-email",
            Phone = "1234567890"
        };

        var results = Validate(req);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateContactRequest.Email)));
    }

    [Fact]
    public void CreateContactRequest_ShortPhone_FailsValidation()
    {
        var req = new CreateContactRequest
        {
            Name = "Joshua",
            Email = "josh@example.com",
            Phone = "12"
        };

        var results = Validate(req);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateContactRequest.Phone)));
    }

    private static List<ValidationResult> Validate(object obj)
    {
        var ctx = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(obj, ctx, results, validateAllProperties: true);
        return results;
    }
}
