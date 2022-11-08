namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.When_posting_a_new_vereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Constants;
using AutoFixture;
using Events;
using Fixtures;
using FluentAssertions;
using Framework.Helpers;
using Marten;
using Xunit;

[Collection(VerenigingAdminApiCollection.Name)]
public class Given_A_Valid_Request_With_All_Fields
{
    private readonly VerenigingAdminApiFixture _apiFixture;
    private readonly Fixture _fixture;

    public Given_A_Valid_Request_With_All_Fields(VerenigingAdminApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Then_it_returns_an_accepted_response()
    {
        var request = new
        {
            Naam = _fixture.Create<string>(),
            KorteNaam = _fixture.Create<string>(),
            KorteBeschrijving = _fixture.Create<string>(),
            Startdatum = DateOnly.FromDateTime(DateTime.Today),
            KboNummer = "0123456789",
        };
        var content = GetJsonBody(request).AsJsonContent();
        var response = await _apiFixture.HttpClient!.PostAsync("/v1/verenigingen", content);
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Then_it_saves_the_events()
    {
        var request = new
        {
            Naam = _fixture.Create<string>(),
            KorteNaam = _fixture.Create<string>(),
            KorteBeschrijving = _fixture.Create<string>(),
            Startdatum = DateOnly.FromDateTime(DateTime.Today),
            KboNummer = "0123456789",
        };
        var content = GetJsonBody(request).AsJsonContent();
        await _apiFixture.HttpClient!.PostAsync("/v1/verenigingen", content);

        var savedEvent = await _apiFixture.DocumentStore!
            .LightweightSession().Events
            .QueryRawEventDataOnly<VerenigingWerdGeregistreerd>()
            .SingleAsync(e => e.Naam == request.Naam);
        savedEvent.KorteNaam.Should().Be(request.KorteNaam);
        savedEvent.KorteBeschrijving.Should().Be(request.KorteBeschrijving);
        savedEvent.Startdatum.Should().Be(request.Startdatum.ToDateTime(TimeOnly.MinValue));
        savedEvent.KboNummer.Should().Be(request.KboNummer);
        savedEvent.Status.Should().Be("Actief");
    }

    private string GetJsonBody(dynamic request)
        => GetType()
            .GetAssociatedResourceJson($"files.request.with_all_fields")
            .Replace("{{vereniging.naam}}", (string)request.Naam)
            .Replace("{{vereniging.korteNaam}}", (string)request.KorteNaam)
            .Replace("{{vereniging.korteBeschrijving}}", (string)request.KorteBeschrijving)
            .Replace("{{vereniging.startdatum}}", ((DateOnly)request.Startdatum).ToString(WellknownFormats.DateOnly))
            .Replace("{{vereniging.kboNummer}}", (string)request.KboNummer);
}
