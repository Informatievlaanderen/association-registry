namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.When_posting_a_new_vereniging;

using System.Net;
using System.Text;
using AutoFixture;
using Events;
using Fixtures;
using FluentAssertions;
using Xunit;

[Collection(VerenigingAdminApiCollection.Name)]
public class Given_An_Api : IDisposable
{
    private readonly VerenigingAdminApiFixture _apiFixture;
    private readonly Fixture _fixture;

    public Given_An_Api(VerenigingAdminApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Given_an_empty_Naam_Then_it_returns_a_xxx()
    {
        var content = GetContent(string.Empty);
        var response = await _apiFixture.HttpClient.PostAsync("/v1/verenigingen", content);
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Then_it_returns_an_accepted_response()
    {
        var content = GetContent(_fixture.Create<string>());
        var response = await _apiFixture.HttpClient.PostAsync("/v1/verenigingen", content);
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Then_it_saves_the_events()
    {
        var expectedNaam = _fixture.Create<string>();
        var content = GetContent(expectedNaam);
        await _apiFixture.HttpClient.PostAsync("/v1/verenigingen", content);

        _apiFixture.DocumentStore.LightweightSession().Events.QueryRawEventDataOnly<VerenigingWerdGeregistreerd>()
            .Where(e => e.Naam == expectedNaam)
            .Should().HaveCount(1);
    }

    private StringContent GetContent(string naam)
        => new(
            GetJsonBody(naam),
            Encoding.UTF8,
            "application/json");

    private string GetJsonBody(string naam)
        => GetType()
            .GetAssociatedResourceJson($"{nameof(Given_An_Api)}_{nameof(Then_it_returns_an_accepted_response)}")
            .Replace("{{vereniging.naam}}", naam);

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
