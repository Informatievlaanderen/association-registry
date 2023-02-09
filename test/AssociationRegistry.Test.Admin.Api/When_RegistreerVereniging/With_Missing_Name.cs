namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging;

using System.Net;
using Fixtures;
using Framework;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerVereniging_With_Missing_Naam
{
    public readonly HttpResponseMessage Response;
    private string GetJsonRequestBody()
        => GetType().GetAssociatedResourceJson("files.request.with_missing_naam");
    private When_RegistreerVereniging_With_Missing_Naam(AdminApiFixture fixture)
    {
        Response ??= fixture.DefaultClient.RegistreerVereniging(GetJsonRequestBody()).GetAwaiter().GetResult();
    }

    private static When_RegistreerVereniging_With_Missing_Naam? called;
    public static When_RegistreerVereniging_With_Missing_Naam Called(AdminApiFixture fixture)
        => called ??= new When_RegistreerVereniging_With_Missing_Naam(fixture);
}

[Collection(nameof(AdminApiCollection))]
[System.ComponentModel.Category("AdminApi")]
[IntegrationTest]
public class With_Missing_Name
{
    private readonly EventsInDbScenariosFixture _fixture;
    private string GetJsonResponseBody()
        => GetType()
            .GetAssociatedResourceJson($"files.response.missing_name_error");

    private HttpResponseMessage Response
        => When_RegistreerVereniging_With_Missing_Naam.Called(_fixture).Response;

    public With_Missing_Name(EventsInDbScenariosFixture fixture)
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

        var responseContentObject = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        var expectedResponseContentObject = JsonConvert.DeserializeObject<ValidationProblemDetails>(GetJsonResponseBody());

        responseContentObject.Should().BeEquivalentTo(
            expectedResponseContentObject,
            options => options
                .Excluding(info => info!.ProblemInstanceUri)
                .Excluding(info => info!.ProblemTypeUri));
    }
}
