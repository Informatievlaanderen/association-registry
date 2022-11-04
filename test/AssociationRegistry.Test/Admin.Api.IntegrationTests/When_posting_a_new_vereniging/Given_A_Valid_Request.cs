namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.When_posting_a_new_vereniging;

using System.Net;
using AutoFixture;
using Events;
using Fixtures;
using FluentAssertions;
using Framework.Helpers;
using Xunit;

[Collection(VerenigingAdminApiCollection.Name)]
public class Given_A_Valid_Request
{
    private readonly VerenigingAdminApiFixture _apiFixture;
    private readonly Fixture _fixture;

    public Given_A_Valid_Request(VerenigingAdminApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Then_it_returns_an_accepted_response()
    {
        string naam = _fixture.Create<string>();
        var content = GetJsonBody(naam).AsJsonContent();
        var response = await _apiFixture.HttpClient!.PostAsync("/v1/verenigingen", content);
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Then_it_saves_the_events()
    {
        var expectedNaam = _fixture.Create<string>();
        var content = GetJsonBody(expectedNaam).AsJsonContent();
        await _apiFixture.HttpClient!.PostAsync("/v1/verenigingen", content);

        _apiFixture.DocumentStore!.LightweightSession().Events.QueryRawEventDataOnly<VerenigingWerdGeregistreerd>()
            .Where(e => e.Naam == expectedNaam)
            .Should().HaveCount(1);
    }

    private string GetJsonBody(string naam)
        => GetType()
            .GetAssociatedResourceJson($"files.request.with_name")
            .Replace("{{vereniging.naam}}", naam);
}
