namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingZonderEigenRechtspersoonlijkheid.Publiek.MutatieDienst;

using Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using Public.Api.Verenigingen.Mutaties;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using FluentAssertions;
using Xunit;

[Collection(WellKnownCollections.RegistreerVerenigingZonderEigenRechtspersoonlijkheid)]
public class Returns_VerenigingMutationsSequenceResponse : End2EndTest<RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext, RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest, PubliekVerenigingSequenceResponse[]>
{
    private readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext _testContext;

    public Returns_VerenigingMutationsSequenceResponse(RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext testContext) : base(testContext)
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

    public override Func<IApiSetup, PubliekVerenigingSequenceResponse[]> GetResponse
        => setup => setup.PublicApiHost.GetVerenigingMutationsSequence();
}
