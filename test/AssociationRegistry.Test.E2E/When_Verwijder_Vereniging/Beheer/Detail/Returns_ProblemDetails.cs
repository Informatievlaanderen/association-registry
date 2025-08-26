namespace AssociationRegistry.Test.E2E.When_Verwijder_Vereniging.Detail;

using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;

[Collection(nameof(VerwijderVerenigingCollection))]
public class Returns_ProblemDetails : End2EndTest<ProblemDetails>
{
    private readonly VerwijderVerenigingContext _context;

    public Returns_ProblemDetails(VerwijderVerenigingContext context) : base(context.ApiSetup)
    {
        _context = context;
    }

    [Fact]
    public async ValueTask With_VerenigingWerdVerwijderd_In_Response()
    {
        Response!.Title.Should().Be("Er heeft zich een fout voorgedaan!");
        Response!.Detail.Should().Be("Deze vereniging werd verwijderd.");
        Response.ProblemTypeUri.Should().Be("urn:associationregistry.admin.api:validation");
    }

    public override async Task<ProblemDetails> GetResponse(FullBlownApiSetup setup)
        => await _context.ApiSetup.AdminApiHost.GetProblemDetailsForBeheerDetailHttpResponse(_context.ApiSetup.SuperAdminHttpClient, _context.VCode, _context.MaxSequenceByScenario.Value);
}
