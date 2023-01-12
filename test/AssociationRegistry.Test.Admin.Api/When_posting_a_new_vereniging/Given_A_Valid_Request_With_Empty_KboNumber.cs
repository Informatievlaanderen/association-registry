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
        var response = await _apiFixture.AdminApiClient.RegistreerVereniging(_apiFixture.JsonContent);
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

}
