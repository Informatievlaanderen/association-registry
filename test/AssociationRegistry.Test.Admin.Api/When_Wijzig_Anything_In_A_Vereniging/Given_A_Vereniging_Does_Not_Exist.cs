namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Anything_In_A_Vereniging;

using System.Net;
using Fixtures;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_A_Vereniging_Does_Not_Exist
{
    private const string VCode = "V9999999";
    private const string NieuweVerenigingsNaam = "De nieuwe vereniging";
    private readonly HttpResponseMessage _response;

    public Given_A_Vereniging_Does_Not_Exist(EventsInDbScenariosFixture fixture)
    {
        var jsonBody = $@"{{""naam"":""{NieuweVerenigingsNaam}""}}";
        _response = fixture.DefaultClient.PatchVereniging(VCodes.VCode.Create(VCode), jsonBody).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_it_returns_a_bad_request_response()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
