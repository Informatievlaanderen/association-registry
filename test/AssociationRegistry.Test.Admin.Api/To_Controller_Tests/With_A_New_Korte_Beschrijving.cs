namespace AssociationRegistry.Test.Admin.Api.To_Controller_Tests;

using System.Net;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.VCodes;
using AutoFixture;
using FluentAssertions;
using Xunit;

//TODO rework into controller test
public class With_A_New_Korte_Beschrijving_Fixture : AdminApiFixture
{
    public HttpResponseMessage Response = null!;
    private readonly string _vCode;
    private readonly Fixture _fixture;
    public const string NieuweKorteBeschrijving = "De nieuwe korte beschrijving";

    public With_A_New_Korte_Beschrijving_Fixture() : base(
        nameof(With_A_New_Korte_Beschrijving_Fixture))
    {
        _fixture = new Fixture().CustomizeAll();
        _vCode = _fixture.Create<VCode>();
    }

    protected override async Task Given()
    {
        await AddEvent(
            _vCode,
            _fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = _vCode },
            _fixture.Create<CommandMetadata>()
        );
    }

    protected override async Task When()
    {
        var jsonBody = $@"{{""korteBeschrijving"":""{NieuweKorteBeschrijving}"", ""Initiator"": ""OVO000001""}}";
        Response = await AdminApiClient.PatchVereniging(_vCode, jsonBody);
    }
}

public class With_A_New_Korte_Beschrijving : IClassFixture<With_A_New_Korte_Beschrijving_Fixture>
{
    private readonly With_A_New_Korte_Beschrijving_Fixture _apiFixture;

    public With_A_New_Korte_Beschrijving(With_A_New_Korte_Beschrijving_Fixture apiFixture)
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
        using var session = _apiFixture.DocumentStore
            .LightweightSession();
        var savedEvents = session.Events
            .QueryRawEventDataOnly<KorteBeschrijvingWerdGewijzigd>()
            .ToList();

        savedEvents.Should().HaveCount(1);
        savedEvents[0].KorteBeschrijving.Should().Be(With_A_New_Korte_Beschrijving_Fixture.NieuweKorteBeschrijving);
    }
}
