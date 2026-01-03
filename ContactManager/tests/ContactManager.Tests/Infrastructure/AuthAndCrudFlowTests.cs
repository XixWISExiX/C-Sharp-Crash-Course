using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ContactManager.Contracts.Contacts;
using ContactManager.Tests.Infrastructure;
//using Xunit;

namespace ContactManager.Tests.Integration;

public sealed class AuthAndCrudFlowTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthAndCrudFlowTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task WithoutToken_ContactsEndpointsReturn401()
    {
        var res = await _client.GetAsync("/contacts");
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task WithToken_CanCreateGetUpdateDeleteContact()
    {
        // 1) Login
        var loginRes = await _client.PostAsJsonAsync("/auth/login", new { username = "admin", password = "password" });
        loginRes.EnsureSuccessStatusCode();

        var login = await loginRes.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(login);
        Assert.False(string.IsNullOrWhiteSpace(login!.AccessToken));

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", login.AccessToken);

        // 2) Create
        var createReq = new CreateContactRequest { Name = "Joshua", Email = "josh@example.com", Phone = "1234567890" };
        var createRes = await _client.PostAsJsonAsync("/contacts", createReq);
        Assert.Equal(HttpStatusCode.Created, createRes.StatusCode);

        var created = await createRes.Content.ReadFromJsonAsync<ContactDto>();
        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created!.Id);

        // 3) Get
        var getRes = await _client.GetAsync($"/contacts/{created.Id}");
        getRes.EnsureSuccessStatusCode();
        var got = await getRes.Content.ReadFromJsonAsync<ContactDto>();
        Assert.Equal("Joshua", got!.Name);

        // 4) Update
        var updateReq = new UpdateContactRequest { Name = "Joshua Wiseman", Email = "josh@example.com", Phone = "1234567890" };
        var updateRes = await _client.PutAsJsonAsync($"/contacts/{created.Id}", updateReq);
        updateRes.EnsureSuccessStatusCode();
        var updated = await updateRes.Content.ReadFromJsonAsync<ContactDto>();
        Assert.Equal("Joshua Wiseman", updated!.Name);

        // 5) Delete
        var delRes = await _client.DeleteAsync($"/contacts/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delRes.StatusCode);

        // 6) Confirm gone
        var getAfterDel = await _client.GetAsync($"/contacts/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfterDel.StatusCode);
    }

    private sealed record LoginResponse(string AccessToken, string TokenType, int ExpiresInSeconds);
}
