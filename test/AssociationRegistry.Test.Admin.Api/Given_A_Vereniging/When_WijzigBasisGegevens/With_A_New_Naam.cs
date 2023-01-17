namespace AssociationRegistry.Test.Admin.Api.Given_A_Vereniging.When_WijzigBasisGegevens;

using System.Net;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Fixtures;
using AutoFixture;
using FluentAssertions;
using Marten;
using NodaTime.Extensions;
using Xunit;

public class With_A_Naam_Fixture : AdminApiFixture2
{
    public HttpResponseMessage Response = null!;
    public const string VCode = "v0001001";
    public const string NieuweVerenigingsNaam = "De nieuwe vereniging";
    private const string JsonBody = $@"{{""naam"":""{NieuweVerenigingsNaam}""}}";

    public With_A_Naam_Fixture() : base(
        nameof(With_A_Naam_Fixture))
    {
    }

    protected override async Task Given()
    {
        var fixture = new Fixture();
        await AddEvent(
            VCode,
            new VerenigingWerdGeregistreerd(
                VCode,
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                DateOnly.FromDateTime(fixture.Create<DateTime>()),
                fixture.Create<string>(),
                Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                DateOnly.FromDateTime(DateTime.Today)),
            new CommandMetadata(
                fixture.Create<string>(),
                new DateTime(2022, 1, 1).ToUniversalTime().ToInstant())
        );
    }

    protected override async Task When()
    {
        Response = await AdminApiClient.PatchVereniging(VCode, JsonBody);
    }
}

public class With_A_Naam : IClassFixture<With_A_Naam_Fixture>
{
    private readonly With_A_Naam_Fixture _apiFixture;

    public With_A_Naam(With_A_Naam_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _apiFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public void Then_it_saves_the_events()
    {
        var savedEvents = _apiFixture.DocumentStore
            .LightweightSession().Events
            .QueryRawEventDataOnly<NaamWerdGewijzigd>()
            .ToList();

        savedEvents.Should().HaveCount(1);
        savedEvents[0].Naam.Should().Be(With_A_Naam_Fixture.NieuweVerenigingsNaam);
    }
}
