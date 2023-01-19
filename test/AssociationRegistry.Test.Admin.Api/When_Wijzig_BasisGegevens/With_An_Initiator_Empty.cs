namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_BasisGegevens;

using System.Net;
using Fixtures;
using FluentAssertions;
using Xunit;

public class With_An_Initiator_Empty_Fixture : AdminApiFixture2
{
    private const string VCode = "V0001001";

    public With_An_Initiator_Empty_Fixture() : base(
        nameof(With_An_Initiator_Empty_Fixture))
    {}

    protected override Task Given()
        => Task.CompletedTask;

    protected override async Task When()
    {
        Response = await AdminApiClient.PatchVereniging(VCode, @"{ ""initiator"": """"}");
    }

    public HttpResponseMessage Response { get; private set; } = null!;
}

public class With_An_Initiator_Empty : IClassFixture<With_An_Initiator_Empty_Fixture>
{
    private readonly With_An_Initiator_Empty_Fixture _apiFixture;

    public With_An_Initiator_Empty(With_An_Initiator_Empty_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public void Then_it_returns_a_bad_request_response()
    {
        _apiFixture.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
