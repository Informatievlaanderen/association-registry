namespace AssociationRegistry.Test.Admin.Api.Given_A_Vereniging.When_WijzigBasisGegevens;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Fixtures;
using AutoFixture;
using FluentAssertions;
using NodaTime.Extensions;
using Xunit;

public class With_The_Same_Naam_Fixture : AdminApiFixture
{
    private readonly Fixture _fixture;
    public VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;
    public const string VCode = "V0001001";

    public With_The_Same_Naam_Fixture() : base(
        nameof(With_The_Same_Naam_Fixture))
    {
        _fixture = new Fixture();
    }

    public override async Task InitializeAsync()
    {
        VerenigingWerdGeregistreerd = new VerenigingWerdGeregistreerd(
            VCode,
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            DateOnly.FromDateTime(_fixture.Create<DateTime>()),
            _fixture.Create<string>(),
            Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
            Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
            DateOnly.FromDateTime(DateTime.Today));
        await AddEvent(
            VCode,
            VerenigingWerdGeregistreerd,
            new CommandMetadata(
                _fixture.Create<string>(),
                new DateTime(2022, 1, 1).ToUniversalTime().ToInstant()));
    }
}

public class With_The_Same_Naam : IClassFixture<With_The_Same_Naam_Fixture>
{
    private readonly With_The_Same_Naam_Fixture _apiFixture;

    public With_The_Same_Naam(With_The_Same_Naam_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Then_it_returns_an_accepted_response()
    {
        var jsonBody = $@"{{""naam"":""{_apiFixture.VerenigingWerdGeregistreerd.Naam}"", ""Initiator"": ""OVO000001""}}";
        var response = await _apiFixture.AdminApiClient.PatchVereniging(With_The_Same_Naam_Fixture.VCode, jsonBody);
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Then_it_returns_no_sequence_header()
    {
        var jsonBody = $@"{{""naam"":""{_apiFixture.VerenigingWerdGeregistreerd.Naam}"", ""Initiator"": ""OVO000001""}}";
        var response = await _apiFixture.AdminApiClient.PatchVereniging(With_The_Same_Naam_Fixture.VCode, jsonBody);
        response.Headers.Should().NotContainKey(WellknownHeaderNames.Sequence);
    }
}
