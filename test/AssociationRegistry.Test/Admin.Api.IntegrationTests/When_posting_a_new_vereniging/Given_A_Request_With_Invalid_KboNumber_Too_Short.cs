namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.When_posting_a_new_vereniging;

using System.Net;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Fixtures;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

public class Given_A_Request_With_Invalid_KboNumber_Too_Short_Fixture : JsonRequestAdminApiFixture
{
    public Given_A_Request_With_Invalid_KboNumber_Too_Short_Fixture() : base(
        nameof(Given_A_Request_With_Invalid_KboNumber_Too_Short_Fixture),
        "files.request.with_invalid_kbonummer_too_short")
    {
    }
}

public class Given_A_Request_With_Invalid_KboNumber_Too_Short : IClassFixture<Given_A_Request_With_Invalid_KboNumber_Too_Short_Fixture>
{
    private readonly Given_A_Request_With_Invalid_KboNumber_Too_Short_Fixture _apiFixture;

    public Given_A_Request_With_Invalid_KboNumber_Too_Short(Given_A_Request_With_Invalid_KboNumber_Too_Short_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Then_it_returns_a_badrequest_response()
    {
        var response = await _apiFixture.AdminApiClient.RegistreerVereniging(_apiFixture.Content);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Then_it_returns_a_problemdetails_response()
    {
        var response = await _apiFixture.AdminApiClient.RegistreerVereniging(_apiFixture.Content);

        var responseContent = await response.Content.ReadAsStringAsync();

        var responseContentObject = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        var expectedResponseContentObject = JsonConvert.DeserializeObject<ValidationProblemDetails>(GetJsonResponseBody());

        responseContentObject.Should().BeEquivalentTo(
            expectedResponseContentObject,
            options => options
                .Excluding(info => info!.ProblemInstanceUri)
                .Excluding(info => info!.ProblemTypeUri));
    }

    private string GetJsonResponseBody()
        => GetType()
            .GetAssociatedResourceJson($"files.response.invalid_kbonummer_too_short_validation_error");
}
