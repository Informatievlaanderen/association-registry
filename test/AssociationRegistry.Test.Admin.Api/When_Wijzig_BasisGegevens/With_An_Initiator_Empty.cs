namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_BasisGegevens;

using System.Net;
using Fixtures;
using FluentAssertions;
using Xunit;

public class With_An_Initiator_Empty_Fixture : AdminApiFixture
{
    public const string VCode = "V0001001";

    public With_An_Initiator_Empty_Fixture() : base(
        nameof(With_An_Initiator_Empty_Fixture))
    {}
}

public class With_An_Initiator_Empty : IClassFixture<With_An_Initiator_Empty_Fixture>
{
    private readonly With_An_Initiator_Empty_Fixture _apiFixture;
    private const string JsonBody = @"{ ""initiator"": """"}";

    public With_An_Initiator_Empty(With_An_Initiator_Empty_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Then_it_returns_a_bad_request_response()
    {
        var response = await _apiFixture.AdminApiClient.PatchVereniging(With_An_Initiator_Empty_Fixture.VCode, JsonBody);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
