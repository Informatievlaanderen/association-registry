namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingZonderEigenRechtspersoonlijkheid.Publiek.MutatieDienst;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Public.Api.WebApi.Verenigingen.Mutaties;
using Xunit;

[Collection(nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCollection))]
public class Returns_MutatieDienstResponse : End2EndTest<PubliekVerenigingSequenceResponse[]>
{
    private readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext _testContext;

    public Returns_MutatieDienstResponse(RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<PubliekVerenigingSequenceResponse[]> GetResponse(FullBlownApiSetup setup)
        => await setup.PublicApiHost.GetVerenigingMutationsSequence();

    [Fact]
    public void With_RegisteredVereniging()
    {
        var actual = Response.SingleOrDefault(x => x.VCode == _testContext.VCode);
        actual.Should().NotBeNull();
        actual!.Sequence.Should().BeGreaterThan(0);
    }
}
