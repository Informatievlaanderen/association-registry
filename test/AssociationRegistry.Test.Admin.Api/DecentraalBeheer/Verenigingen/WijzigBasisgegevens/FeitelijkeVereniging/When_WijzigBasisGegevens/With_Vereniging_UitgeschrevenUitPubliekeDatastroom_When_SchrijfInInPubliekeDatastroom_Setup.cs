namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.When_WijzigBasisGegevens;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Events;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using AutoFixture;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Xunit;
using Xunit.Categories;

public sealed class With_Vereniging_UitgeschrevenUitPubliekeDatastroom_When_SchrijfInInPubliekeDatastroom_Setup
{
    public WijzigBasisgegevensRequest Request { get; }
    public V021_FeitelijkeVerenigingWerdGeregistreerdAsUitgeschrevenUitPubliekeDatastroomScenario Scenario { get; }
    public HttpResponseMessage Response { get; }

    public With_Vereniging_UitgeschrevenUitPubliekeDatastroom_When_SchrijfInInPubliekeDatastroom_Setup(EventsInDbScenariosFixture fixture)
    {
        Scenario = fixture.V021FeitelijkeVerenigingWerdGeregistreerdAsUitgeschrevenUitPubliekeDatastroom;

        Request = new Fixture().CustomizeAdminApi().Create<WijzigBasisgegevensRequest>();

        var jsonBody = @"{
            ""isUitgeschrevenUitPubliekeDatastroom"":false,
            ""initiator"": ""OVO000001""}";

        Response = fixture.DefaultClient.PatchVereniging(Scenario.VCode, jsonBody).GetAwaiter().GetResult();
    }
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_Vereniging_UitgeschrevenUitPubliekeDatastroom_When_SchrijfInInPubliekeDatastroom :
    IClassFixture<With_Vereniging_UitgeschrevenUitPubliekeDatastroom_When_SchrijfInInPubliekeDatastroom_Setup>
{
    private readonly HttpResponseMessage _response;
    private readonly string _vCode;
    private readonly IDocumentStore _documentStore;
    private readonly AppSettings _appSettings;

    public With_Vereniging_UitgeschrevenUitPubliekeDatastroom_When_SchrijfInInPubliekeDatastroom(
        With_Vereniging_UitgeschrevenUitPubliekeDatastroom_When_SchrijfInInPubliekeDatastroom_Setup setup,
        EventsInDbScenariosFixture fixture)
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

        session.Events
               .FetchStream(_vCode)
               .Single(@event => @event.Data.GetType() == typeof(VerenigingWerdIngeschrevenInPubliekeDatastroom))
               .Should().NotBeNull();
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
