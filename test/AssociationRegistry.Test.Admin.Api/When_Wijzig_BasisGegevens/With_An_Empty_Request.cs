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
    public const string VCode = "V0001001";

    public With_An_Empty_Request_Fixture() : base(
        nameof(With_An_Empty_Request_Fixture))
    {}
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
    public async Task Then_it_returns_a_bad_request_response()
    {
        var response = await _apiFixture.AdminApiClient.PatchVereniging(With_An_Empty_Request_Fixture.VCode, JsonBody);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
