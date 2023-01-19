namespace AssociationRegistry.Test.Admin.Api.Given_A_Vereniging.When_WijzigBasisGegevens;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using Events;
using AssociationRegistry.Framework;
using Fixtures;
using AutoFixture;
using FluentAssertions;
using Framework;
using Microsoft.Extensions.DependencyInjection;
using VCodes;
using Xunit;

public class With_A_New_Korte_Naam_Fixture : AdminApiFixture2
{
    public HttpResponseMessage Response = null!;
    private readonly string _vCode;
    private readonly Fixture _fixture;
    public const string NieuweKorteNaam = "De nieuwe korte naam";

    public With_A_New_Korte_Naam_Fixture() : base(
        nameof(With_A_New_Korte_Naam_Fixture))
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
        var jsonBody = $@"{{""korteNaam"":""{NieuweKorteNaam}"", ""Initiator"": ""OVO000001""}}";
        Response = await AdminApiClient.PatchVereniging(_vCode, jsonBody);
    }
}

public class With_A_New_Korte_Naam : IClassFixture<With_A_New_Korte_Naam_Fixture>
{
    private readonly With_A_New_Korte_Naam_Fixture _apiFixture;

    public With_A_New_Korte_Naam(With_A_New_Korte_Naam_Fixture apiFixture)
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
            .QueryRawEventDataOnly<KorteNaamWerdGewijzigd>()
            .ToList();

        savedEvents.Should().HaveCount(1);
        savedEvents[0].KorteNaam.Should().Be(With_A_New_Korte_Naam_Fixture.NieuweKorteNaam);
    }
}
