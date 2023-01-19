namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_BasisGegevens;

using System.Net;
using Fixtures;
using FluentAssertions;
using Xunit;

public class With_An_Empty_Request_Fixture : AdminApiFixture2
{
    public const string VCode = "V0001001";

    public With_An_Empty_Request_Fixture() : base(
        nameof(With_An_Empty_Request_Fixture))
    {}

    protected override async Task Given()
    {
    }

    protected override async Task When()
    {
        Response = await AdminApiClient.PatchVereniging(With_An_Empty_Request_Fixture.VCode, "{}");
    }

    public HttpResponseMessage Response { get; set; }
}

public class With_An_Empty_Request : IClassFixture<With_An_Empty_Request_Fixture>
{
    private readonly With_An_Empty_Request_Fixture _apiFixture;

    public With_An_Empty_Request(With_An_Empty_Request_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Then_it_returns_a_bad_request_response()
    {
        _apiFixture.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
