﻿namespace AssociationRegistry.Test.Admin.Api.When_WijzigBasisGegevens;

using System.Net;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using Events;
using AssociationRegistry.Framework;
using Primitives;
using Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Categories;

public sealed class When_WijzigBasisGegevens_WithAllBasisGegevensGewijzigd
{
    public readonly string VCode;
    public readonly WijzigBasisgegevensRequest Request;


    private When_WijzigBasisGegevens_WithAllBasisGegevensGewijzigd(EventsInDbScenariosFixture fixture)
    {
        const string nieuweVerenigingsNaam = "De nieuwe vereniging";
        const string nieuweKorteNaam = "De nieuwe korte naam";
        const string nieuweKorteBeschrijving = "De nieuwe korte beschrijving";
        const string initiator = "OVO000001";
        var nieuweStartdatum = fixture.ServiceProvider.GetRequiredService<IClock>().Today.AddDays(-1);

        Request = new WijzigBasisgegevensRequest
        {
            Naam = nieuweVerenigingsNaam,
            KorteNaam = nieuweKorteNaam,
            KorteBeschrijving = nieuweKorteBeschrijving,
            Initiator = initiator,
            Startdatum = NullOrEmpty<DateOnly>.Create(nieuweStartdatum),
        };
        VCode = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario.VCode;

        var jsonBody = $@"{{
            ""naam"":""{nieuweVerenigingsNaam}"",
            ""korteNaam"":""{nieuweKorteNaam}"",
            ""korteBeschrijving"":""{nieuweKorteBeschrijving}"",
            ""startdatum"":""{nieuweStartdatum.ToString(WellknownFormats.DateOnly)}"",
            ""Initiator"": ""OVO000001""}}";

        Response = fixture.DefaultClient.PatchVereniging(VCode, jsonBody).GetAwaiter().GetResult();
    }

    private static When_WijzigBasisGegevens_WithAllBasisGegevensGewijzigd? called;


    public static When_WijzigBasisGegevens_WithAllBasisGegevensGewijzigd Called(EventsInDbScenariosFixture fixture)
        => called ??= new When_WijzigBasisGegevens_WithAllBasisGegevensGewijzigd(fixture);

    public HttpResponseMessage Response { get; }
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_All_BasisGegevensWerdenGewijzigd
{
    private readonly EventsInDbScenariosFixture _fixture;

    private WijzigBasisgegevensRequest Request
        => When_WijzigBasisGegevens_WithAllBasisGegevensGewijzigd.Called(_fixture).Request;

    private HttpResponseMessage Response
        => When_WijzigBasisGegevens_WithAllBasisGegevensGewijzigd.Called(_fixture).Response;

    private string VCode
        => When_WijzigBasisGegevens_WithAllBasisGegevensGewijzigd.Called(_fixture).VCode;

    public With_All_BasisGegevensWerdenGewijzigd(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Then_it_saves_the_events()
    {
        using var session = _fixture.DocumentStore
            .LightweightSession();
        var naamWerdGewijzigd = session.Events
            .QueryRawEventDataOnly<NaamWerdGewijzigd>()
            .Single(@event => @event.VCode == VCode);
        var korteNaamWerdGewijzigd = session.Events
            .QueryRawEventDataOnly<KorteNaamWerdGewijzigd>()
            .Single(@event => @event.VCode == VCode);
        var korteBeschrijvingWerdGewijzigd = session.Events
            .QueryRawEventDataOnly<KorteBeschrijvingWerdGewijzigd>()
            .Single(@event => @event.VCode == VCode);
        var startdatumWerdGewijzigd = session.Events
            .QueryRawEventDataOnly<StartdatumWerdGewijzigd>()
            .Single(@event => @event.VCode == VCode);


        naamWerdGewijzigd.Naam.Should().Be(Request.Naam);
        korteNaamWerdGewijzigd.KorteNaam.Should().Be(Request.KorteNaam);
        korteBeschrijvingWerdGewijzigd.KorteBeschrijving.Should().Be(Request.KorteBeschrijving);
        startdatumWerdGewijzigd.Startdatum.Should().Be(Request.Startdatum.Value);
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public void Then_it_returns_a_location_header()
    {
        Response.Headers.Should().ContainKey(Microsoft.Net.Http.Headers.HeaderNames.Location);
        Response.Headers.Location!.OriginalString.Should()
            .StartWith($"{_fixture.ServiceProvider.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");
    }

    [Fact]
    public void Then_it_returns_a_sequence_header()
    {
        Response.Headers.Should().ContainKey(WellknownHeaderNames.Sequence);
        var sequenceValues = Response.Headers.GetValues(WellknownHeaderNames.Sequence).ToList();
        sequenceValues.Should().HaveCount(1);
        var sequence = Convert.ToInt64(sequenceValues.Single());
        sequence.Should().BeGreaterThan(0);
    }
}
