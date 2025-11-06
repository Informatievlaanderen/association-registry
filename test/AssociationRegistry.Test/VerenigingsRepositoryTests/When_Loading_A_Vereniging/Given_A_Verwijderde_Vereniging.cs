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
using Resources;
using Xunit;

public class Given_A_Verwijderde_Vereniging
{

    [Theory]
    [InlineData(typeof(FeitelijkeVerenigingWerdGeregistreerd))]
    [InlineData(typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd))]
    public async Task Then_A_FeitelijkeVereniging_Is_Returned(Type verenigingType)
    {
        var fixture = new Fixture().CustomizeDomain();
        var context = new SpecimenContext(fixture);
        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)context.Resolve(verenigingType);

        var eventStoreMock = new Mock<IEventStore>();
        eventStoreMock
           .Setup(x => x.Load(It.IsAny<string>(), It.IsAny<long?>()))
           .ReturnsAsync(new VerenigingState
            {
                IsVerwijderd = true,
                VerenigingStatus = new VerenigingStatus.StatusActief(),
            });

        var repo = new VerenigingsRepository(eventStoreMock.Object);

        var exception = await
            Assert.ThrowsAsync<VerenigingIsVerwijderd>(async () => await repo.Load<Vereniging>(VCode.Create(verenigingWerdGeregistreerd.VCode), TestCommandMetadata.Empty)) ;

        exception.Message.Should().Be(ExceptionMessages.VerenigingIsVerwijderd);
    }
}
