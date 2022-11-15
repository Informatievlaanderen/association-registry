namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.When_posting_a_new_vereniging;

using System.Net;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Fixtures;
using FluentAssertions;
using Framework.Helpers;
using Newtonsoft.Json;
using Xunit;

[Collection(VerenigingAdminApiCollection.Name)]
public class Given_A_Request_With_Invalid_KboNumber
{
    private readonly VerenigingAdminApiFixture _apiFixture;

    public Given_A_Request_With_Invalid_KboNumber(VerenigingAdminApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Then_it_returns_a_badrequest_response()
    {
        var content = GetJsonRequestBody().AsJsonContent();
        var response = await _apiFixture.HttpClient!.PostAsync("/v1/verenigingen", content);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Then_it_returns_a_problemdetails_response()
    {
        var content = GetJsonRequestBody().AsJsonContent();
        var response = await _apiFixture.HttpClient!.PostAsync("/v1/verenigingen", content);

        var responseContent = await response.Content.ReadAsStringAsync();

        var responseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(responseContent);
        var expectedResponseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(GetJsonResponseBody());

        responseContentObject.Should().BeEquivalentTo(
            expectedResponseContentObject,
            options => options
                .Excluding(info => info!.ProblemInstanceUri));
    }

    private string GetJsonRequestBody()
        => GetType()
            .GetAssociatedResourceJson($"files.request.with_invalid_kbonummer");

    private string GetJsonResponseBody()
        => GetType()
            .GetAssociatedResourceJson($"files.response.invalid_kbonummer_validation_error");
}
