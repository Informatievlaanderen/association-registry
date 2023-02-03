namespace AssociationRegistry.Test.Admin.Api.TakeTwo.Given_A_Vereniging_Does_Not_Exist.When_registreer_a_new_vereniging;

using System.Net;
using Framework;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerVereniging_With_Missing_Naam {
    private HttpResponseMessage? _response;

    private string GetJsonRequestBody()
        => GetType().GetAssociatedResourceJson("files.request.with_missing_naam");
    private When_RegistreerVereniging_With_Missing_Naam()
    {
    }

    private static When_RegistreerVereniging_With_Missing_Naam? called;
    public static When_RegistreerVereniging_With_Missing_Naam Called
        => called ??= new When_RegistreerVereniging_With_Missing_Naam();

    public HttpResponseMessage Response(AdminApiFixture2 fixture)
        => _response ??= fixture.DefaultClient.RegistreerVereniging(GetJsonRequestBody()).GetAwaiter().GetResult();
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
        => When_RegistreerVereniging_With_Missing_Naam.Called.Response(_fixture);

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
