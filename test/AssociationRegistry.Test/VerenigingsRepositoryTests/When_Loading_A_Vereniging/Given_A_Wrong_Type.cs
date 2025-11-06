namespace AssociationRegistry.Test.VerenigingsRepositoryTests.When_Loading_A_Vereniging;

using AutoFixture;
using AutoFixture.Kernel;
using Common.AutoFixture;
using Common.Framework;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using Events;
using FluentAssertions;
using MartenDb.Store;
using Moq;
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

        var eventStoreMock = new Mock<IEventStore>();
        eventStoreMock
           .Setup(x => x.Load(It.IsAny<string>(), It.IsAny<long?>()))
           .ReturnsAsync(new VerenigingState
            {
                IsVerwijderd = false,
                VerenigingStatus = new VerenigingStatus.StatusActief(),
            });

        var repo = new VerenigingsRepository(eventStoreMock.Object);
        var loadMethod = () => repo.Load<VerenigingMetRechtspersoonlijkheid>(VCode.Create(verenigingWerdGeregistreerd.VCode), TestCommandMetadata.Empty);
        await loadMethod.Should().ThrowAsync<ActieIsNietToegestaanVoorVerenigingstype>();
    }
}
