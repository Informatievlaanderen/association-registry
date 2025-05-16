namespace AssociationRegistry.Test.E2E.When_Verwijder_Vereniging.Beheer.Detail;

using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Framework.AlbaHost;
using Newtonsoft.Json;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_ProblemDetails : IClassFixture<VerwijderVerenigingContext>
{
    private readonly VerwijderVerenigingContext _context;

    public Returns_ProblemDetails(VerwijderVerenigingContext context)
    {
        _context = context;
    }

    [Fact]
    public async Task With_VerenigingWerdVerwijderd_In_Response()
    {
        var response = _context.ApiSetup.AdminApiHost.GetBeheerDetailHttpResponse(_context.ApiSetup.SuperAdminHttpClient, _context.VCode, 2);
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseContentObject = JsonConvert.DeserializeObject<ProblemDetails>(responseContent);

        responseContentObject!.Title.Should().Be("Er heeft zich een fout voorgedaan!");
        responseContentObject!.Detail.Should().Be("Deze vereniging werd verwijderd.");
        responseContentObject.ProblemTypeUri.Should().Be("urn:associationregistry.admin.api:validation");
    }
}
