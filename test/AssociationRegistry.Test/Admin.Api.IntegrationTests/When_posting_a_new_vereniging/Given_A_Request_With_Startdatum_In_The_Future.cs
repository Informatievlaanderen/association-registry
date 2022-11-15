namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.When_posting_a_new_vereniging;

using System.Net;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Fixtures;
using FluentAssertions;
using Framework.Helpers;
using Newtonsoft.Json;
using Xunit;

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
    public async Task Then_it_returns_a_problemdetails_response()
    {
        var content = GetJsonRequestBody(Startdatum).AsJsonContent();
        var response = await _apiFixture.HttpClient!.PostAsync("/v1/verenigingen", content);

        var responseContent = await response.Content.ReadAsStringAsync();

        var responseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(responseContent);
        var expectedResponseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(GetJsonResponseBody());

        responseContentObject.Should().BeEquivalentTo(
            expectedResponseContentObject,
            options => options
                .Excluding(info => info!.ProblemInstanceUri)
                .Excluding(info => info!.ProblemTypeUri));
    }

    private string GetJsonRequestBody(DateOnly startdatum)
        => GetType()
            .GetAssociatedResourceJson($"files.request.with_startdatum_in_future")
            .Replace("{{vereniging.startdatum}}", startdatum.ToString("yyyy-MM-dd"));

    private string GetJsonResponseBody()
        => GetType()
            .GetAssociatedResourceJson($"files.response.startdatum_in_future_validation_error");
}
