namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.EventFactories;
using AssociationRegistry.Events;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.Framework;
using FluentAssertions;
using JasperFx.Core;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Net;
using Xunit;

public sealed class When_WijzigBasisGegevens_WithAllBasisGegevensGewijzigd_Setup
{
    public WijzigBasisgegevensRequest Request { get; }
    public V015_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithAllFields_ForWijzigBasisgegevens Scenario { get; }
    public HttpResponseMessage Response { get; }

    public When_WijzigBasisGegevens_WithAllBasisGegevensGewijzigd_Setup(EventsInDbScenariosFixture fixture)
    {
        Scenario = fixture.V015VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAllFieldsForWijzigBasisgegevens;

        Request = new Fixture().CustomizeAdminApi().Create<WijzigBasisgegevensRequest>();

        var jsonBody = $@"{{
            ""roepnaam"":""{Request.Roepnaam}"",
            ""korteBeschrijving"":""{Request.KorteBeschrijving}"",
            ""doelgroep"": {{
                ""minimumleeftijd"": {Request.Doelgroep!.Minimumleeftijd!},
                ""maximumleeftijd"": {Request.Doelgroep!.Maximumleeftijd!}
            }},
            ""hoofdactiviteitenVerenigingsloket"":[{Request.HoofdactiviteitenVerenigingsloket!.Select(h => $@"""{h}""").Join(",")}],
            ""werkingsgebieden"":[{Request.Werkingsgebieden!.Select(h => $@"""{h}""").Join(",")}]
            }}";

        Response = fixture.DefaultClient.PatchVerenigingMetRechtspersoonlijkheid(Scenario.VCode, jsonBody).GetAwaiter().GetResult();
    }
}

[Collection(nameof(AdminApiCollection))]
public class With_All_BasisGegevensWerdenGewijzigd : IClassFixture<When_WijzigBasisGegevens_WithAllBasisGegevensGewijzigd_Setup>
{
    private readonly WijzigBasisgegevensRequest _request;
    private readonly HttpResponseMessage _response;
    private readonly string _vCode;
    private readonly IDocumentStore _documentStore;
    private readonly AppSettings _appSettings;

    public With_All_BasisGegevensWerdenGewijzigd(
        When_WijzigBasisGegevens_WithAllBasisGegevensGewijzigd_Setup setup,
        EventsInDbScenariosFixture fixture)
    {
        _request = setup.Request;
        _response = setup.Response;
        _vCode = setup.Scenario.VCode;
        _documentStore = fixture.DocumentStore;
        _appSettings = fixture.ServiceProvider.GetRequiredService<AppSettings>();
    }

    [Fact]
    public async ValueTask Then_it_saves_the_events()
    {
        var werkingsgebiedenServiceMock = new WerkingsgebiedenServiceMock();

        await using var session = _documentStore
           .LightweightSession();

        var events = await session.Events
                            .FetchStreamAsync(_vCode);

        var roepnaamWerdGewijzigd = events.Single(e => e.Data.GetType() == typeof(RoepnaamWerdGewijzigd));
        roepnaamWerdGewijzigd.Data.Should().BeEquivalentTo(new RoepnaamWerdGewijzigd(_request.Roepnaam ?? ""));

        var korteBeschrijvingWerdGewijzigd = events.Single(@event => @event.Data.GetType() == typeof(KorteBeschrijvingWerdGewijzigd));

        korteBeschrijvingWerdGewijzigd.Data.Should()
                                      .BeEquivalentTo(new KorteBeschrijvingWerdGewijzigd(_vCode, _request.KorteBeschrijving!));

        var doelgroepWerdGewijzigd = events.Single(@event => @event.Data.GetType() == typeof(DoelgroepWerdGewijzigd));

        doelgroepWerdGewijzigd.Data.Should()
                              .BeEquivalentTo(new DoelgroepWerdGewijzigd(
                                                  new Registratiedata.Doelgroep(_request.Doelgroep!.Minimumleeftijd!.Value,
                                                                                _request.Doelgroep!.Maximumleeftijd!.Value)));

        var hoofdactiviteitenVerenigingsloketWerdenGewijzigd = events
           .Single(@event => @event.Data.GetType() == typeof(HoofdactiviteitenVerenigingsloketWerdenGewijzigd));

        hoofdactiviteitenVerenigingsloketWerdenGewijzigd.Data.Should().BeEquivalentTo(
            EventFactory.HoofdactiviteitenVerenigingsloketWerdenGewijzigd(_request.HoofdactiviteitenVerenigingsloket!
                                                                          .Select(HoofdactiviteitVerenigingsloket.Create).ToArray()));

        var werkingsgebiedenWerdenGewijzigd = events
           .Single(@event => @event.Data.GetType() == typeof(WerkingsgebiedenWerdenGewijzigd));

        werkingsgebiedenWerdenGewijzigd.Data.Should().BeEquivalentTo(
            EventFactory.WerkingsgebiedenWerdenGewijzigd(
                VCode.Create(_vCode),
                _request.Werkingsgebieden!.Select(werkingsgebiedenServiceMock.Create).ToArray())
        );
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public void Then_it_returns_a_location_header()
    {
        _response.Headers.Should().ContainKey(HeaderNames.Location);

        _response.Headers.Location!.OriginalString.Should()
                 .StartWith($"{_appSettings.BaseUrl}/v1/verenigingen/V");
    }

    [Fact]
    public void Then_it_returns_a_sequence_header()
    {
        _response.Headers.Should().ContainKey(WellknownHeaderNames.Sequence);
        var sequenceValues = _response.Headers.GetValues(WellknownHeaderNames.Sequence).ToList();
        sequenceValues.Should().HaveCount(1);
        var sequence = Convert.ToInt64(sequenceValues.Single());
        sequence.Should().BeGreaterThan(0);
    }
}
