namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using Events;
using FluentAssertions;
using Framework.Fixtures;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Primitives;
using System.Net;
using Xunit;
using Xunit.Categories;

public sealed class When_WijzigBasisGegevens_WithStartdatumLeeg
{
    public readonly string VCode;
    public readonly WijzigBasisgegevensRequest Request;

    private When_WijzigBasisGegevens_WithStartdatumLeeg(EventsInDbScenariosFixture fixture)
    {
        Request = new WijzigBasisgegevensRequest
        {
            Startdatum = NullOrEmpty<DateOnly>.Empty,
        };

        VCode = fixture.V046FeitelijkeVerenigingWerdGeregistreerdForWijzigStartdatum.VCode;

        const string jsonBody = @"{
            ""startdatum"":"""",
            ""Initiator"": ""OVO000001""}";

        Response = fixture.DefaultClient.PatchVereniging(VCode, jsonBody).GetAwaiter().GetResult();
    }

    private static When_WijzigBasisGegevens_WithStartdatumLeeg? called;

    public static When_WijzigBasisGegevens_WithStartdatumLeeg Called(EventsInDbScenariosFixture fixture)
        => called ??= new When_WijzigBasisGegevens_WithStartdatumLeeg(fixture);

    public HttpResponseMessage Response { get; }
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_StartdatumWerdGewijzigd_Leeg
{
    private readonly EventsInDbScenariosFixture _fixture;

    private HttpResponseMessage Response
        => When_WijzigBasisGegevens_WithStartdatumLeeg.Called(_fixture).Response;

    private string VCode
        => When_WijzigBasisGegevens_WithStartdatumLeeg.Called(_fixture).VCode;

    public With_StartdatumWerdGewijzigd_Leeg(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Then_it_saves_the_events()
    {
        using var session = _fixture.DocumentStore
                                    .LightweightSession();

        var startdatumWerdGewijzigd = session.Events
                                             .QueryRawEventDataOnly<StartdatumWerdGewijzigd>()
                                             .Single(@event => @event.VCode == VCode);

        startdatumWerdGewijzigd.Startdatum.Should().BeNull();
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public void Then_it_returns_a_location_header()
    {
        Response.Headers.Should().ContainKey(HeaderNames.Location);

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
