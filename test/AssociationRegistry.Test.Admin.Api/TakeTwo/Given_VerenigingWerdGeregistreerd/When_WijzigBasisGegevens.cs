namespace AssociationRegistry.Test.Admin.Api.TakeTwo.Given_VerenigingWerdGeregistreerd;

using System.Net;
using Events;
using FluentAssertions;
using Xunit;

[Collection(nameof(AdminApiCollection))]
public class When_WijzigBasisGegevens
{
    private readonly GivenEventsFixture _fixture;
    private const string NieuweVerenigingsNaam = "De nieuwe vereniging";
    private const string NieuweKorteNaam = "De nieuwe korte naam";
    private const string NieuweKorteBeschrijving = "De nieuwe korte beschrijving";
    private readonly HttpResponseMessage _response;
    private readonly string _vCode;

    public When_WijzigBasisGegevens(GivenEventsFixture fixture)
    {
        _fixture = fixture;
        _vCode = fixture.VerenigingWerdGeregistreerdScenario.VCode;
        const string jsonBody = $@"{{
            ""naam"":""{NieuweVerenigingsNaam}"",
            ""korteNaam"":""{NieuweKorteNaam}"",
            ""korteBeschrijving"":""{NieuweKorteBeschrijving}"",
             ""Initiator"": ""OVO000001""}}";
        _response = fixture.AdminApiClient.PatchVereniging(_vCode, jsonBody).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_it_saves_the_events()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.Accepted);

        using var session = _fixture.DocumentStore
            .LightweightSession();
        var naamWerdGewijzigd = session.Events
            .QueryRawEventDataOnly<NaamWerdGewijzigd>()
            .Single(@event => @event.VCode == _vCode);
        var korteNaamWerdGewijzigd = session.Events
            .QueryRawEventDataOnly<KorteNaamWerdGewijzigd>()
            .Single(@event => @event.VCode == _vCode);
        var korteBeschrijvingWerdGewijzigd = session.Events
            .QueryRawEventDataOnly<KorteBeschrijvingWerdGewijzigd>()
            .Single(@event => @event.VCode == _vCode);


        naamWerdGewijzigd.Naam.Should().Be(NieuweVerenigingsNaam);
        korteNaamWerdGewijzigd.KorteNaam.Should().Be(NieuweKorteNaam);
        korteBeschrijvingWerdGewijzigd.KorteBeschrijving.Should().Be(NieuweKorteBeschrijving);
    }
}
