namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_WijzigBasisGegevens;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Fixtures;
using AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Xunit;
using Xunit.Categories;

public sealed class When_NaamGewijzigd_Setup
{
    public WijzigBasisgegevensRequest Request { get; }
    public V047_AfdelingWerdGeregistreerd_MetBestaandeMoeder_ForNaamWijzigen Scenario { get; }
    public HttpResponseMessage Response { get; }

    public When_NaamGewijzigd_Setup(EventsInDbScenariosFixture fixture)
    {
        Scenario = fixture.V047AfdelingWerdGeregistreerdMetBestaandeMoederForNaamWijzigen;

        Request = new Fixture().CustomizeAdminApi().Create<WijzigBasisgegevensRequest>();

        var jsonBody = $@"{{
            ""naam"":""{Request.Naam}""
            }}";

        Response = fixture.DefaultClient.PatchVereniging(Scenario.VCode, jsonBody).GetAwaiter().GetResult();
    }
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_NaamGewijzigd : IClassFixture<When_NaamGewijzigd_Setup>
{
    private readonly WijzigBasisgegevensRequest _request;
    private readonly HttpResponseMessage _response;
    private readonly string _vCode;
    private readonly IDocumentStore _documentStore;
    private readonly AppSettings _appSettings;

    public With_NaamGewijzigd(
        When_NaamGewijzigd_Setup setup,
        EventsInDbScenariosFixture fixture)
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

        var events = session.Events
                            .FetchStream(_vCode);

        var naamWerdGewijzigd = events.Single(e => e.Data.GetType() == typeof(NaamWerdGewijzigd));
        naamWerdGewijzigd.Data.Should().BeEquivalentTo(new NaamWerdGewijzigd(_vCode, _request.Naam!));
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
