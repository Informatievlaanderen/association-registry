namespace AssociationRegistry.Test.Admin.Api.TakeTwo.Given_A_Vereniging_Does_Not_Exist;

using System.Net;
using AssociationRegistry.Test.Admin.Api.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.VCodes;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class When_WijzigBasisGegevens
{
    private const string VCode = "V9999999";
    private const string NieuweVerenigingsNaam = "De nieuwe vereniging";
    private readonly HttpResponseMessage _response;

    public When_WijzigBasisGegevens(GivenEventsFixture fixture)
    {
        var jsonBody = $@"{{""naam"":""{NieuweVerenigingsNaam}""}}";
        _response = fixture.AdminApiClient.PatchVereniging(VCodes.VCode.Create(VCode), jsonBody).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_it_returns_a_bad_request_response()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
