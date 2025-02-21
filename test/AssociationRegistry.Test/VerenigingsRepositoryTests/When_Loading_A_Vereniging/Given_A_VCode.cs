namespace AssociationRegistry.Test.VerenigingsRepositoryTests.When_Loading_A_Vereniging;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Test.Framework;
using AssociationRegistry.Test.Framework.Customizations;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using AutoFixture.Kernel;
using Common.AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
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

        var feteitelijkeVerenging = await repo.Load<Vereniging>(VCode.Create(verenigingWerdGeregistreerd.VCode), expectedVersion: null);

        feteitelijkeVerenging
           .Should()
           .NotBeNull()
           .And
           .BeOfType<Vereniging>();
    }
}
