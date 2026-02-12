namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging;

using System.Net;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

public sealed class When_RegistreerFeitelijkeVereniging_With_Unparsable_Request
{
    private static When_RegistreerFeitelijkeVereniging_With_Unparsable_Request? called;
    public readonly HttpResponseMessage Response;

    private When_RegistreerFeitelijkeVereniging_With_Unparsable_Request(AdminApiFixture fixture)
    {
        Response ??= fixture
            .DefaultClient.RegistreerFeitelijkeVereniging(content: GetJsonRequestBody())
            .GetAwaiter()
            .GetResult();
    }

    private string GetJsonRequestBody() =>
        GetType().GetAssociatedResourceJson(resourceName: "files.request.with_unparsable_request");

    public static When_RegistreerFeitelijkeVereniging_With_Unparsable_Request Called(AdminApiFixture fixture) =>
        called ??= new When_RegistreerFeitelijkeVereniging_With_Unparsable_Request(fixture: fixture);
}

[Collection(name: nameof(AdminApiCollection))]
public class With_Unparsable_Request
{
    private readonly EventsInDbScenariosFixture _fixture;

    public With_Unparsable_Request(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    private HttpResponseMessage Response =>
        When_RegistreerFeitelijkeVereniging_With_Unparsable_Request.Called(fixture: _fixture).Response;

    private string GetJsonResponseBody() =>
        GetType().GetAssociatedResourceJson(resourceName: "files.response.unparsable");

    [Fact]
    public void Then_it_returns_a_badrequest_response()
    {
        Response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
    }

    [Fact]
    public async ValueTask Then_it_returns_a_validationproblemdetails_response()
    {
        var responseContent = await Response.Content.ReadAsStringAsync();

        var responseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(value: responseContent);
        var expectedResponseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(value: GetJsonResponseBody());

        responseContentObject
            .Should()
            .BeEquivalentTo(
                expectation: expectedResponseContentObject,
                config: options =>
                    options
                        .Excluding(expression: info => info!.ProblemInstanceUri)
                        .Excluding(expression: info => info!.ProblemTypeUri)
            );
    }
}
