namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.When_posting_a_new_vereniging;

using System.Net;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Fixtures;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

public class Given_A_Request_With_Missing_Name_Fixture : JsonRequestAdminApiFixture
{
    public Given_A_Request_With_Missing_Name_Fixture() : base(
        nameof(Given_A_Request_With_Missing_Name_Fixture),
        "files.request.with_missing_name")
    {
    }
}

public class Given_A_Request_With_Missing_Name : IClassFixture<Given_A_Request_With_Missing_Name_Fixture>
{
    private readonly Given_A_Request_With_Missing_Name_Fixture _apiFixture;

    public Given_A_Request_With_Missing_Name(Given_A_Request_With_Missing_Name_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Then_it_returns_a_badrequest_response()
    {
        var response = await _apiFixture.HttpClient.PostAsync("/v1/verenigingen", _apiFixture.Content);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Then_it_returns_a_validationproblemdetails_response()
    {
        var response = await _apiFixture.HttpClient.PostAsync("/v1/verenigingen", _apiFixture.Content);

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
            .GetAssociatedResourceJson($"files.response.missing_name_error");
}
