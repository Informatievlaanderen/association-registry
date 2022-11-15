namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.When_posting_a_new_vereniging;

using System.Net;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Fixtures;
using FluentAssertions;
using Framework.Helpers;
using Newtonsoft.Json;
using Xunit;

[Collection(VerenigingAdminApiCollection.Name)]
public class Given_A_Request_With_Missing_Name
{
    private readonly VerenigingAdminApiFixture _apiFixture;

    public Given_A_Request_With_Missing_Name(VerenigingAdminApiFixture apiFixture)
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
    public async Task Then_it_returns_a_validationproblemdetails_response()
    {
        var content = GetJsonRequestBody().AsJsonContent();
        var response = await _apiFixture.HttpClient!.PostAsync("/v1/verenigingen", content);

        var responseContent = await response.Content.ReadAsStringAsync();

        var responseContentObject = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        var expectedResponseContentObject = JsonConvert.DeserializeObject<ValidationProblemDetails>(GetJsonResponseBody());

        responseContentObject.Should().BeEquivalentTo(expectedResponseContentObject, options => options
            .Excluding(info => info!.ProblemInstanceUri)
            .Excluding(info=>info!.ProblemTypeUri));    }

    private string GetJsonRequestBody()
        => GetType()
            .GetAssociatedResourceJson($"files.request.empty");

    private string GetJsonResponseBody()
        => GetType()
            .GetAssociatedResourceJson($"files.response.not_null_validation_error");
}

[Collection(VerenigingAdminApiCollection.Name)]
public class Given_A_Request_With_Startdatum_In_The_Future
{
    private readonly VerenigingAdminApiFixture _apiFixture;
    private static readonly DateOnly Startdatum;

    public Given_A_Request_With_Startdatum_In_The_Future(VerenigingAdminApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    static Given_A_Request_With_Startdatum_In_The_Future()
    {
        Startdatum = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
    }

    [Fact]
    public async Task Then_it_returns_a_badrequest_response()
    {
        var content = GetJsonRequestBody(Startdatum).AsJsonContent();
        var response = await _apiFixture.HttpClient!.PostAsync("/v1/verenigingen", content);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Then_it_returns_a_validationproblemdetails_response()
    {
        var content = GetJsonRequestBody(Startdatum).AsJsonContent();
        var response = await _apiFixture.HttpClient!.PostAsync("/v1/verenigingen", content);

        var responseContent = await response.Content.ReadAsStringAsync();

        var responseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(responseContent);
        var expectedResponseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(GetJsonResponseBody());

        responseContentObject.Should().BeEquivalentTo(
            expectedResponseContentObject,
            options => options
                .Excluding(info => info!.ProblemInstanceUri));
    }

    private string GetJsonRequestBody(DateOnly startdatum)
        => GetType()
            .GetAssociatedResourceJson($"files.request.with_startdatum_in_future")
            .Replace("{{vereniging.startdatum}}", startdatum.ToString("yyyy-MM-dd"));

    private string GetJsonResponseBody()
        => GetType()
            .GetAssociatedResourceJson($"files.response.startdatum_in_future_validation_error");
}
