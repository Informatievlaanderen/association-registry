namespace AssociationRegistry.Test.Admin.Api.TakeTwo.Given_VerenigingWerdGeregistreerd.When_WijzigBasisGegevens;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Fixtures;
using AutoFixture;
using FluentAssertions;
using NodaTime.Extensions;
using Xunit;
using Xunit.Categories;

public sealed class When_WijzigBasisgegevens_WithSameNaam
{
    public readonly string VCode;
    public readonly WijzigBasisgegevensRequest Request;
    public readonly HttpResponseMessage Response;

    private When_WijzigBasisgegevens_WithSameNaam(EventsInDbScenariosFixture fixture)
    {
        Request = new WijzigBasisgegevensRequest()
        {
            Naam = fixture.VerenigingWerdGeregistreerdForUseWithNoChangesEventsInDbScenario.VerenigingWerdGeregistreerd.Naam,
            Initiator = "OVO000001",
        };
        VCode = fixture.VerenigingWerdGeregistreerdForUseWithNoChangesEventsInDbScenario.VCode;

        var jsonBody =
            $@"{{""naam"":""{Request.Naam}"", ""Initiator"": ""{Request.Initiator}""}}";

        Response = fixture.DefaultClient.PatchVereniging(VCode, jsonBody).GetAwaiter().GetResult();
    }

    private static When_WijzigBasisgegevens_WithSameNaam? called;

    public static When_WijzigBasisgegevens_WithSameNaam Called(EventsInDbScenariosFixture fixture)
        => called ??= new When_WijzigBasisgegevens_WithSameNaam(fixture);
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_The_Same_Naam
{
    private readonly EventsInDbScenariosFixture _fixture;

    private HttpResponseMessage Response
        => When_WijzigBasisgegevens_WithSameNaam.Called(_fixture).Response;

    private string VCode
        => When_WijzigBasisgegevens_WithSameNaam.Called(_fixture).VCode;

    public With_The_Same_Naam(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Then_it_returns_an_ok_response()
    {
        Response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void Then_it_returns_no_sequence_header()
    {
        Response.Headers.Should().NotContainKey(WellknownHeaderNames.Sequence);
    }

    [Fact]
    public void Then_it_returns_no_location_header()
    {
        Response.Headers.Should().NotContainKey(Microsoft.Net.Http.Headers.HeaderNames.Location);
    }

    [Fact]
    public void Then_it_saves_no_events()
    {
        using var session = _fixture.DocumentStore
            .LightweightSession();
        var savedEvents = session.Events
            .QueryRawEventDataOnly<NaamWerdGewijzigd>()
            .SingleOrDefault(@event => @event.VCode == VCode);

        savedEvents.Should().BeNull();
    }
}
