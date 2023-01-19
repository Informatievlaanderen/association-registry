namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_BasisGegevens;

using System.Net;
using Fixtures;
using FluentAssertions;
using Xunit;

public class With_An_Initiator_Null_Fixture : AdminApiFixture
{
    public const string VCode = "V0001001";

    public With_An_Initiator_Null_Fixture() : base(
        nameof(With_An_Initiator_Null_Fixture))
    {}
}

public class With_An_Initiator_Null : IClassFixture<With_An_Initiator_Null_Fixture>
{
    private readonly With_An_Initiator_Null_Fixture _apiFixture;
    private const string JsonBody = @"{ ""initiator"": null}";

    public With_An_Initiator_Null(With_An_Initiator_Null_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Then_it_returns_a_bad_request_response()
    {
        var response = await _apiFixture.AdminApiClient.PatchVereniging(With_An_Initiator_Null_Fixture.VCode, JsonBody);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
