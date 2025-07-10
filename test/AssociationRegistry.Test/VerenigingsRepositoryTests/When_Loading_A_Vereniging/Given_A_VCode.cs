namespace AssociationRegistry.Test.VerenigingsRepositoryTests.When_Loading_A_Vereniging;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Test.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using AutoFixture.Kernel;
using Common.AutoFixture;
using Common.Framework;
using FluentAssertions;
using Xunit;

public class Given_A_VCode
{
    [Theory]
    [InlineData(typeof(FeitelijkeVerenigingWerdGeregistreerd))]
    [InlineData(typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd))]
    public async Task Then_A_FeitelijkeVereniging_Is_Returned(Type verenigingType)
    {
        var fixture = new Fixture().CustomizeDomain();
        var context = new SpecimenContext(fixture);
        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)context.Resolve(verenigingType);

        var eventStoreMock = new EventStoreMock((dynamic)verenigingWerdGeregistreerd);

        var repo = new VerenigingsRepository(eventStoreMock);

        var feteitelijkeVerenging = await repo.Load<Vereniging>(VCode.Create(verenigingWerdGeregistreerd.VCode), TestCommandMetadata.Empty);

        feteitelijkeVerenging
           .Should()
           .NotBeNull()
           .And
           .BeOfType<Vereniging>();
    }
}
