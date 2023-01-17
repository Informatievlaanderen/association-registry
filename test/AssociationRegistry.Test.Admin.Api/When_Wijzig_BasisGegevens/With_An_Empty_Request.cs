namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_BasisGegevens;

using System.Net;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Fixtures;
using AutoFixture;
using FluentAssertions;
using NodaTime.Extensions;
using Xunit;

public class With_An_Empty_Request_Fixture : AdminApiFixture
{
    private readonly Fixture _fixture;
    public const string VCode = "v0001001";

    public With_An_Empty_Request_Fixture() : base(
        nameof(With_An_Empty_Request_Fixture))
    {
        _fixture = new Fixture();
    }

    public override async Task InitializeAsync()
    {
        await AddEvent(
            VCode,
            new VerenigingWerdGeregistreerd(
                VCode,
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                DateOnly.FromDateTime(_fixture.Create<DateTime>()),
                _fixture.Create<string>(),
                Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                DateOnly.FromDateTime(DateTime.Today)),
            new CommandMetadata(
                _fixture.Create<string>(),
                new DateTime(2022, 1, 1).ToUniversalTime().ToInstant()));
    }
}

public class With_An_Empty_Request : IClassFixture<With_An_Empty_Request_Fixture>
{
    private readonly With_An_Empty_Request_Fixture _apiFixture;
    private const string JsonBody = "{}";

    public With_An_Empty_Request(With_An_Empty_Request_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Then_it_returns_a_accepted_request_response()
    {
        var response = await _apiFixture.AdminApiClient.PatchVereniging(With_An_Empty_Request_Fixture.VCode, JsonBody);
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}
