namespace AssociationRegistry.Test.Admin.Api.TakeTwo.Given_A_Vereniging_Does_Not_Exist.When_registreer_a_new_vereniging;

using System.Net;
using AssociationRegistry.Test.Admin.Api.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerVereniging_With_Invalid_Startdatum {
    private HttpResponseMessage? _response;

    private string GetJsonRequestBody()
        => GetType().GetAssociatedResourceJson("files.request.with_an_invalid_startdatum");
    private When_RegistreerVereniging_With_Invalid_Startdatum()
    {
    }

    private static When_RegistreerVereniging_With_Invalid_Startdatum? called;
    public static When_RegistreerVereniging_With_Invalid_Startdatum Called
        => called ??= new When_RegistreerVereniging_With_Invalid_Startdatum();

    public HttpResponseMessage Response(AdminApiFixture2 fixture)
        => _response ??= fixture.DefaultClient.RegistreerVereniging(GetJsonRequestBody()).GetAwaiter().GetResult();
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_An_Invalid_Startdatum
{
    private readonly EventsInDbScenariosFixture _fixture;

    private string GetJsonResponseBody()
        => GetType()
            .GetAssociatedResourceJson($"files.response.invalid_startdatum_validation_error");

    private HttpResponseMessage Response
        => When_RegistreerVereniging_With_Invalid_Startdatum.Called.Response(_fixture);

    public With_An_Invalid_Startdatum(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Then_it_returns_a_badrequest_response()
    {
        Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Then_it_returns_a_problemdetails_response()
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
