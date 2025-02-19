namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging.Publiek.MutatieDienst;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using Public.Api.Verenigingen.Mutaties;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_VerenigingMutationsSequenceResponse : End2EndTest<RegistreerFeitelijkeVerenigingTestContext, RegistreerFeitelijkeVerenigingRequest, PubliekVerenigingSequenceResponse[]>
{
    private readonly RegistreerFeitelijkeVerenigingTestContext _testContext;

    public Returns_VerenigingMutationsSequenceResponse(RegistreerFeitelijkeVerenigingTestContext testContext) : base(testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_RegisteredVereniging()
    {
        var actual = Response.SingleOrDefault(x => x.VCode == _testContext.VCode);
        actual.Should().NotBeNull();
        actual.Sequence.Should().BeGreaterThan(0);
    }

    public override Func<IApiSetup, PubliekVerenigingSequenceResponse[]> GetResponse
        => setup => setup.PublicApiHost.GetVerenigingMutationsSequence();
}
