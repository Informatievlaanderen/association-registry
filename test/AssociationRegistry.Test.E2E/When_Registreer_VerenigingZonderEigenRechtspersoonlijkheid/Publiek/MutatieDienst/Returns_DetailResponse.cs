namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingZonderEigenRechtspersoonlijkheid.Publiek.MutatieDienst;

using AssociationRegistry.Public.Api.Verenigingen.Mutaties;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using Xunit;

[Collection(nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCollection))]
public class Returns_VerenigingMutationsSequenceResponse : End2EndTest<PubliekVerenigingSequenceResponse[]>
{
    private readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext _testContext;

    public Returns_VerenigingMutationsSequenceResponse(RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_RegisteredVereniging()
    {
        var actual = Response.SingleOrDefault(x => x.VCode == _testContext.VCode);
        actual.Should().NotBeNull();
        actual!.Sequence.Should().BeGreaterThan(0);
    }

    public override PubliekVerenigingSequenceResponse[] GetResponse(FullBlownApiSetup setup)
        => setup.PublicApiHost.GetVerenigingMutationsSequence();
}
