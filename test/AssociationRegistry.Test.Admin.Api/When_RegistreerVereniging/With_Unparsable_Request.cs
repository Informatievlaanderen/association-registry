namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging;

using System.Net;
using Fixtures;
using Framework;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerVereniging_With_Unparsable_Request
{
    public readonly HttpResponseMessage Response;
    private string GetJsonRequestBody()
        => GetType().GetAssociatedResourceJson("files.request.with_unparsable_request");
    private When_RegistreerVereniging_With_Unparsable_Request(AdminApiFixture fixture)
    {
        Response ??= fixture.DefaultClient.RegistreerVereniging(GetJsonRequestBody()).GetAwaiter().GetResult();
    }

    private static When_RegistreerVereniging_With_Unparsable_Request? called;
    public static When_RegistreerVereniging_With_Unparsable_Request Called(AdminApiFixture fixture)
        => called ??= new When_RegistreerVereniging_With_Unparsable_Request(fixture);
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_Unparsable_Request
{
    private readonly EventsInDbScenariosFixture _fixture;

    private string GetJsonResponseBody()
        => GetType()
            .GetAssociatedResourceJson($"files.response.unparsable");

    private HttpResponseMessage Response
        => When_RegistreerVereniging_With_Unparsable_Request.Called(_fixture).Response;

    public With_Unparsable_Request(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Then_it_returns_a_badrequest_response()
    {
        Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Then_it_returns_a_validationproblemdetails_response()
    {
        var responseContent = await Response.Content.ReadAsStringAsync();

        var responseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(responseContent);
        var expectedResponseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(GetJsonResponseBody());

        responseContentObject.Should().BeEquivalentTo(
            expectedResponseContentObject,
            options => options
                .Excluding(info => info!.ProblemInstanceUri)
                .Excluding(info => info!.ProblemTypeUri));
    }
}
