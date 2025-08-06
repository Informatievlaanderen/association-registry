namespace AssociationRegistry.Test.Admin.Api.Migrate_To_E2E.When_Wijzig_Anything_In_A_Vereniging;

using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using FluentAssertions;
using System.ComponentModel;
using System.Net;
using Xunit;

[Collection(nameof(AdminApiCollection))]
[Category(Categories.MoveToBasicE2E)]
public class Given_A_Vereniging_Does_Not_Exist
{
    private const string VCode = "V9999999";
    private const string NieuweVerenigingsNaam = "De nieuwe vereniging";
    private readonly HttpResponseMessage _response;

    public Given_A_Vereniging_Does_Not_Exist(EventsInDbScenariosFixture fixture)
    {
        var jsonBody = $@"{{""naam"":""{NieuweVerenigingsNaam}""}}";
        _response = fixture.DefaultClient.PatchVereniging(AssociationRegistry.DecentraalBeheer.Vereniging.VCode.Create(VCode), jsonBody).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_it_returns_a_bad_request_response()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
