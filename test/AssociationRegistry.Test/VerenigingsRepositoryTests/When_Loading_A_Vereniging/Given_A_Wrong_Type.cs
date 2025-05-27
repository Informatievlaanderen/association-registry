namespace AssociationRegistry.Test.VerenigingsRepositoryTests.When_Loading_A_Vereniging;

using AssociationRegistry.EventStore;
using AutoFixture;
using AutoFixture.Kernel;
using Common.AutoFixture;
using Common.Framework;
using Events;
using FluentAssertions;
using Framework;
using Repositories;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;

public class Given_A_Wrong_Type
{
    [Theory]
    [InlineData(typeof(FeitelijkeVerenigingWerdGeregistreerd))]
    [InlineData(typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd))]
    public async Task Then_It_Throws_A_UnsupportedOperationException(Type verenigingType)
    {
        var fixture = new Fixture().CustomizeDomain();
        var context = new SpecimenContext(fixture);

        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)context.Resolve(verenigingType);

        var eventStoreMock = new EventStoreMock(
            (dynamic)verenigingWerdGeregistreerd);

        var repo = new VerenigingsRepository(eventStoreMock);
        var loadMethod = () => repo.Load<VerenigingMetRechtspersoonlijkheid>(VCode.Create(verenigingWerdGeregistreerd.VCode), TestCommandMetadata.Empty);
        await loadMethod.Should().ThrowAsync<ActieIsNietToegestaanVoorVerenigingstype>();
    }
}
