namespace AssociationRegistry.Test.Admin.Api.Given_A_Vereniging_Does_Not_Exist.When_posting_a_new_vereniging;

using System.Net;
using Fixtures;
using Framework;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

public class Given_An_Invalid_Request_With_An_Invalid_Startdatum_Fixture : JsonRequestAdminApiFixture
{
    public Given_An_Invalid_Request_With_An_Invalid_Startdatum_Fixture() : base(
        nameof(Given_An_Invalid_Request_With_An_Invalid_Startdatum_Fixture),
        "files.request.with_an_invalid_startdatum")
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

public class Given_An_Invalid_Request_With_An_Invalid_Startdatum: IClassFixture<Given_An_Invalid_Request_With_An_Invalid_Startdatum_Fixture>
{
    private readonly Given_An_Invalid_Request_With_An_Invalid_Startdatum_Fixture _apiFixture;

    public Given_An_Invalid_Request_With_An_Invalid_Startdatum(Given_An_Invalid_Request_With_An_Invalid_Startdatum_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public void Then_it_returns_a_badrequest_response()
    {
        _apiFixture.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Then_it_returns_a_problemdetails_response()
    {
        var response = await _apiFixture.AdminApiClient.RegistreerVereniging(_apiFixture.JsonContent);

        var responseContent = await response.Content.ReadAsStringAsync();

        var responseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(responseContent);
        var expectedResponseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(GetJsonResponseBody());

        responseContentObject.Should().BeEquivalentTo(
            expectedResponseContentObject,
            options => options
                .Excluding(info => info!.ProblemInstanceUri)
                .Excluding(info => info!.ProblemTypeUri));
    }

    private string GetJsonResponseBody()
        => GetType()
            .GetAssociatedResourceJson($"files.response.invalid_startdatum_validation_error");
}
