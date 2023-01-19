namespace AssociationRegistry.Test.Admin.Api.When_posting_a_new_vereniging;

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

    protected override async Task Given()
    {
    }

    protected override async Task When()
    {
        Response = await AdminApiClient.RegistreerVereniging(JsonContent);
    }

    public HttpResponseMessage Response { get; private set; }
}

public class Given_A_Valid_Request_With_Empty_KboNumber : IClassFixture<Given_A_Valid_Request_With_Empty_KboNumber_Fixture>
{
    private readonly Given_A_Valid_Request_With_Empty_KboNumber_Fixture _apiFixture;

    public Given_A_Valid_Request_With_Empty_KboNumber(Given_A_Valid_Request_With_Empty_KboNumber_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Then_it_returns_an_ok_response()
    {
        _apiFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

}
