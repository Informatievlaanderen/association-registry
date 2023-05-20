﻿namespace AssociationRegistry.Test.Admin.Api.When_WijzigBasisGegevens;

using System.Net;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using Events;
using AssociationRegistry.Framework;
using AutoFixture;
using Primitives;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
using JasperFx.Core;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Vereniging;
using Xunit;
using Xunit.Categories;

public sealed class When_WijzigBasisGegevens_WithAllBasisGegevensGewijzigd_Setup
{
    public WijzigBasisgegevensRequest Request { get; }
    public V014_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields_ForWijzigBasisgegevens Scenario { get; }
    public HttpResponseMessage Response { get; }

    public When_WijzigBasisGegevens_WithAllBasisGegevensGewijzigd_Setup(EventsInDbScenariosFixture fixture)
    {
        Scenario = fixture.V014FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForWijzigBasisgegevens;

        Request = new Fixture().CustomizeAll().Create<WijzigBasisgegevensRequest>();

        var startdatumInHetVerleden = fixture.ServiceProvider.GetRequiredService<IClock>().Today.AddDays(-1);
        Request.Startdatum = NullOrEmpty<DateOnly>.Create(startdatumInHetVerleden);

        var jsonBody = $@"{{
            ""naam"":""{Request.Naam}"",
            ""korteNaam"":""{Request.KorteNaam}"",
            ""korteBeschrijving"":""{Request.KorteBeschrijving}"",
            ""startdatum"":""{Request.Startdatum.Value.ToString(WellknownFormats.DateOnly)}"",
            ""hoofdactiviteitenVerenigingsloket"":[{Request.HoofdactiviteitenVerenigingsloket!.Select(h => $@"""{h}""").Join(",")}],
            ""initiator"": ""OVO000001""}}";

        Response = fixture.DefaultClient.PatchVereniging(Scenario.VCode, jsonBody).GetAwaiter().GetResult();
    }
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_All_BasisGegevensWerdenGewijzigd : IClassFixture<When_WijzigBasisGegevens_WithAllBasisGegevensGewijzigd_Setup>
{
    private readonly WijzigBasisgegevensRequest _request;
    private readonly HttpResponseMessage _response;
    private readonly string _vCode;
    private readonly IDocumentStore _documentStore;
    private readonly AppSettings _appSettings;

    public With_All_BasisGegevensWerdenGewijzigd(When_WijzigBasisGegevens_WithAllBasisGegevensGewijzigd_Setup setup, EventsInDbScenariosFixture fixture)
    {
        _request = setup.Request;
        _response = setup.Response;
        _vCode = setup.Scenario.VCode;
        _documentStore = fixture.DocumentStore;
        _appSettings = fixture.ServiceProvider.GetRequiredService<AppSettings>();
    }

    [Fact]
    public void Then_it_saves_the_events()
    {
        using var session = _documentStore
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
        var startdatumWerdGewijzigd = session.Events
            .QueryRawEventDataOnly<StartdatumWerdGewijzigd>()
            .Single(@event => @event.VCode == _vCode);
        var hoofactiviteitenVerenigingloketWerdenGewijzigd = session.Events
            .FetchStream(_vCode)
            .Single(@event => @event.Data.GetType() == typeof(HoofdactiviteitenVerenigingsloketWerdenGewijzigd));


        naamWerdGewijzigd.Naam.Should().Be(_request.Naam);
        korteNaamWerdGewijzigd.KorteNaam.Should().Be(_request.KorteNaam);
        korteBeschrijvingWerdGewijzigd.KorteBeschrijving.Should().Be(_request.KorteBeschrijving);
        startdatumWerdGewijzigd.Startdatum.Should().Be(_request.Startdatum.Value);
        hoofactiviteitenVerenigingloketWerdenGewijzigd.Data.Should().BeEquivalentTo(
            HoofdactiviteitenVerenigingsloketWerdenGewijzigd.With(_request.HoofdactiviteitenVerenigingsloket!.Select(HoofdactiviteitVerenigingsloket.Create).ToArray()));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public void Then_it_returns_a_location_header()
    {
        _response.Headers.Should().ContainKey(Microsoft.Net.Http.Headers.HeaderNames.Location);
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
