namespace AssociationRegistry.Test.Admin.Api.Given_A_Vereniging_Does_Not_Exist.When_posting_a_new_vereniging;

using System.Net;
using Fixtures;
using Framework;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

//TODO Move to TakeTwo as soon as we have a solution for one call with multiple tests in test class
public class Given_An_Unparsable_Request_Fixture : JsonRequestAdminApiFixture
{
    public Given_An_Unparsable_Request_Fixture() : base(
        nameof(Given_An_Unparsable_Request_Fixture),
        "files.request.with_unparsable_request")
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

public class Given_An_Unparsable_Request : IClassFixture<Given_An_Unparsable_Request_Fixture>
{
    private readonly Given_An_Unparsable_Request_Fixture _apiFixture;

    public Given_An_Unparsable_Request(Given_An_Unparsable_Request_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public void Then_it_returns_a_badrequest_response()
    {
        _apiFixture.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Then_it_returns_a_validationproblemdetails_response()
    {
        var responseContent = await _apiFixture.Response.Content.ReadAsStringAsync();

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
            .GetAssociatedResourceJson($"files.response.unparsable");
}
