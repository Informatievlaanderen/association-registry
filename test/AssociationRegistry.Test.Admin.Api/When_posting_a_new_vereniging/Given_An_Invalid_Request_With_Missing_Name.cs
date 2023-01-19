namespace AssociationRegistry.Test.Admin.Api.When_posting_a_new_vereniging;

using System.Net;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Fixtures;
using FluentAssertions;
using Framework;
using Newtonsoft.Json;
using Xunit;

public class Given_An_Invalid_Request_With_Missing_Name_Fixture : JsonRequestAdminApiFixture
{
    public Given_An_Invalid_Request_With_Missing_Name_Fixture() : base(
        nameof(Given_An_Invalid_Request_With_Missing_Name_Fixture),
        "files.request.with_missing_name")
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

public class Given_An_Invalid_Request_With_Missing_Name : IClassFixture<Given_An_Invalid_Request_With_Missing_Name_Fixture>
{
    private readonly Given_An_Invalid_Request_With_Missing_Name_Fixture _apiFixture;

    public Given_An_Invalid_Request_With_Missing_Name(Given_An_Invalid_Request_With_Missing_Name_Fixture apiFixture)
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
