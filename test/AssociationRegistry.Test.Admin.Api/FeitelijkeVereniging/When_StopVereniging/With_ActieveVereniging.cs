namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_StopVereniging;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure;
using Events;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Formats;
using Hosts.Configuration.ConfigurationBindings;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Net;
using Xunit;
using Xunit.Categories;

public sealed class With_ActieveVereniging_Setup
{
    public V040_FeitelijkeVerenigingWerdGeregistreerd_ForStoppen Scenario { get; }
    public HttpResponseMessage Response { get; }

    public With_ActieveVereniging_Setup(EventsInDbScenariosFixture fixture)
    {
        Scenario = fixture.V040FeitelijkeVerenigingWerdGeregistreerdForStoppen;

        var jsonBody = @"{""einddatum"":""2020-12-31""}";

        Response = fixture.DefaultClient.StopVereniging(Scenario.VCode, jsonBody).GetAwaiter().GetResult();
    }
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_ActieveVereniging : IClassFixture<With_ActieveVereniging_Setup>
{
    private readonly HttpResponseMessage _response;
    private readonly string _vCode;
    private readonly IDocumentStore _documentStore;
    private readonly AppSettings _appSettings;

    public With_ActieveVereniging(With_ActieveVereniging_Setup setup, EventsInDbScenariosFixture fixture)
    {
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

        var verenigingWerdGestopt = session.Events
                                           .FetchStream(_vCode)
                                           .Single(@event => @event.Data.GetType() == typeof(VerenigingWerdGestopt));

        (verenigingWerdGestopt.Data as VerenigingWerdGestopt)!.Einddatum.Should()
                                                              .Be(DateOnly.ParseExact(s: "2020-12-31", WellknownFormats.DateOnly));
    }

    [Fact]
    public async Task Then_it_returns_an_accepted_response()
        => _response.StatusCode.Should().Be(HttpStatusCode.Accepted, await _response.Content.ReadAsStringAsync());

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
