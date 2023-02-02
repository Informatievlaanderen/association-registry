namespace AssociationRegistry.Test.Admin.Api.TakeTwo.Given_VerenigingWerdGeregistreerd.When_WijzigBasisGegevens;

using System.Net;
using Events;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_All_BasisGegevensWerdenGewijzigd
{
    private readonly EventsInDbScenariosFixture _fixture;
    private const string NieuweVerenigingsNaam = "De nieuwe vereniging";
    private const string NieuweKorteNaam = "De nieuwe korte naam";
    private const string NieuweKorteBeschrijving = "De nieuwe korte beschrijving";
    private readonly HttpResponseMessage _response;
    private readonly string _vCode;

    public With_All_BasisGegevensWerdenGewijzigd(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
        _vCode = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario.VCode;
        const string jsonBody = $@"{{
            ""naam"":""{NieuweVerenigingsNaam}"",
            ""korteNaam"":""{NieuweKorteNaam}"",
            ""korteBeschrijving"":""{NieuweKorteBeschrijving}"",
             ""Initiator"": ""OVO000001""}}";
        _response = fixture.DefaultClient.PatchVereniging(_vCode, jsonBody).GetAwaiter().GetResult();
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
