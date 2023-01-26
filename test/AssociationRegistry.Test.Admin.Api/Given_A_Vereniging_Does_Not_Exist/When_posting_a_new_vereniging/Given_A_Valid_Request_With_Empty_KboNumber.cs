namespace AssociationRegistry.Test.Admin.Api.Given_A_Vereniging_Does_Not_Exist.When_posting_a_new_vereniging;

using System.Net;
using Fixtures;
using FluentAssertions;
using Xunit;

public class Given_A_Valid_Request_With_Empty_KboNumber_Fixture : JsonRequestAdminApiFixture
{
    public Given_A_Valid_Request_With_Empty_KboNumber_Fixture() : base(
        nameof(Given_A_Valid_Request_With_Empty_KboNumber_Fixture),
        "files.request.with_empty_kbonummer")
    {
    }

    protected override Task Given()
        => Task.CompletedTask;

    protected override async Task When()
    {
        Response = await AdminApiClient.RegistreerVereniging(JsonContent);
    }

    public HttpResponseMessage Response { get; private set; } = null!;
}

public class Given_A_Valid_Request_With_Empty_KboNumber : IClassFixture<Given_A_Valid_Request_With_Empty_KboNumber_Fixture>
{
    private readonly Given_A_Valid_Request_With_Empty_KboNumber_Fixture _apiFixture;

    public Given_A_Valid_Request_With_Empty_KboNumber(Given_A_Valid_Request_With_Empty_KboNumber_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public void Then_it_returns_an_ok_response()
    {
        _apiFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

}
