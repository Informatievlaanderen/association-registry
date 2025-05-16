namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid;

using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using Xunit;
using Xunit.Categories;

public sealed class RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Unparsable_Request
{
    private static RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Unparsable_Request? called;
    public readonly HttpResponseMessage Response;

    private RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Unparsable_Request(AdminApiFixture fixture)
    {
        Response ??= fixture.DefaultClient.RegistreerVerenigingZonderEigenRechtspersoonlijkheid(GetJsonRequestBody()).GetAwaiter().GetResult();
    }

    private string GetJsonRequestBody()
        => GetType().GetAssociatedResourceJson("files.request.with_unparsable_request");

    public static RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Unparsable_Request Called(AdminApiFixture fixture)
        => called ??= new RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Unparsable_Request(fixture);
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_Unparsable_Request
{
    private readonly EventsInDbScenariosFixture _fixture;

    public With_Unparsable_Request(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    private HttpResponseMessage Response
        => RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Unparsable_Request.Called(_fixture).Response;

    private string GetJsonResponseBody()
        => GetType()
           .GetAssociatedResourceJson("files.response.unparsable");

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
            config: options => options
                              .Excluding(info => info!.ProblemInstanceUri)
                              .Excluding(info => info!.ProblemTypeUri));
    }
}
