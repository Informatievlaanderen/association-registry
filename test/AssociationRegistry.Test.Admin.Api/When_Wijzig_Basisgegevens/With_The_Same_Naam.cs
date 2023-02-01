namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Basisgegevens;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Fixtures;
using AutoFixture;
using FluentAssertions;
using NodaTime.Extensions;
using Xunit;

//TODO rework into controller test
public class With_The_Same_Naam_Fixture : AdminApiFixture
{
    private readonly Fixture _fixture;
    private VerenigingWerdGeregistreerd _verenigingWerdGeregistreerd = null!;
    private const string VCode = "V0001001";

    public With_The_Same_Naam_Fixture() : base(
        nameof(With_The_Same_Naam_Fixture))
    {
        _fixture = new Fixture();
    }

    protected override async Task Given()
    {
        _verenigingWerdGeregistreerd = new VerenigingWerdGeregistreerd(
            VCode,
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            DateOnly.FromDateTime(_fixture.Create<DateTime>()),
            _fixture.Create<string>(),
            Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
            Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
            Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>());
        await AddEvent(
            VCode,
            _verenigingWerdGeregistreerd,
            new CommandMetadata(
                _fixture.Create<string>(),
                new DateTime(2022, 1, 1).ToUniversalTime().ToInstant()));
    }

    protected override async Task When()
    {
        var jsonBody = $@"{{""naam"":""{_verenigingWerdGeregistreerd.Naam}"", ""Initiator"": ""OVO000001""}}";
        Response = await AdminApiClient.PatchVereniging(VCode, jsonBody);
    }

    public HttpResponseMessage Response { get; private set; } = null!;
}

public class With_The_Same_Naam : IClassFixture<With_The_Same_Naam_Fixture>
{
    private readonly With_The_Same_Naam_Fixture _apiFixture;

    public With_The_Same_Naam(With_The_Same_Naam_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public void Then_it_returns_an_ok_response()
    {
        _apiFixture.Response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void Then_it_returns_no_sequence_header()
    {
        _apiFixture.Response.Headers.Should().NotContainKey(WellknownHeaderNames.Sequence);
    }
}
