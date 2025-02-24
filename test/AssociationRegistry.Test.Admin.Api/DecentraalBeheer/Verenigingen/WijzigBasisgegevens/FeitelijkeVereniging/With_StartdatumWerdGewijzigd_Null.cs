namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Events;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Primitives;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Net;
using Xunit;
using Xunit.Categories;

public sealed class When_WijzigBasisGegevens_WithStartdatumNull
{
    public readonly string VCode;
    public readonly WijzigBasisgegevensRequest Request;

    private When_WijzigBasisGegevens_WithStartdatumNull(EventsInDbScenariosFixture fixture)
    {
        Request = new WijzigBasisgegevensRequest
        {
            Startdatum = NullOrEmpty<DateOnly>.Empty,
        };

        VCode = fixture.V010FeitelijkeVerenigingWerdGeregistreerdWithAllFields.VCode;

        const string jsonBody = @"{
            ""startdatum"": null,
            ""korteNaam"": ""iets"",
            ""Initiator"": ""OVO000001""}";

        Response = fixture.DefaultClient.PatchVereniging(VCode, jsonBody).GetAwaiter().GetResult();
    }

    private static When_WijzigBasisGegevens_WithStartdatumNull? called;

    public static When_WijzigBasisGegevens_WithStartdatumNull Called(EventsInDbScenariosFixture fixture)
        => called ??= new When_WijzigBasisGegevens_WithStartdatumNull(fixture);

    public HttpResponseMessage Response { get; }
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_StartdatumWerdGewijzigd_Null
{
    private readonly EventsInDbScenariosFixture _fixture;

    private HttpResponseMessage Response
        => When_WijzigBasisGegevens_WithStartdatumNull.Called(_fixture).Response;

    private string VCode
        => When_WijzigBasisGegevens_WithStartdatumNull.Called(_fixture).VCode;

    public With_StartdatumWerdGewijzigd_Null(EventsInDbScenariosFixture fixture)
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
                                             .SingleOrDefault(@event => @event.VCode == VCode);

        startdatumWerdGewijzigd.Should().BeNull();
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
