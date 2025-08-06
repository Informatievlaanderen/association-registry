namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging.Publiek.MutatieDienst;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Public.Api.WebApi.Verenigingen.Mutaties;
using Xunit;

[Collection(nameof(RegistreerFeitelijkeVerenigingCollection))]
public class Returns_Vereniging : End2EndTest<PubliekVerenigingSequenceResponse[]>
{
    private readonly RegistreerFeitelijkeVerenigingContext _testContext;

    public Returns_Vereniging(RegistreerFeitelijkeVerenigingContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override PubliekVerenigingSequenceResponse[] GetResponse(FullBlownApiSetup setup)
        => setup.PublicApiHost.GetVerenigingMutationsSequence();

    [Fact]
    public void With_RegisteredVereniging()
    {
        var actual = Response.SingleOrDefault(x => x.VCode == _testContext.VCode);
        actual.Should().NotBeNull();
        actual!.Sequence.Should().BeGreaterThan(0);
    }
}
