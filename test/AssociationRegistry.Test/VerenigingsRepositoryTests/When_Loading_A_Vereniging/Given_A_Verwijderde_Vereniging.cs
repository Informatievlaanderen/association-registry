namespace AssociationRegistry.Test.VerenigingsRepositoryTests.When_Loading_A_Vereniging;

using AssociationRegistry.EventStore;
using AutoFixture;
using AutoFixture.Kernel;
using Common.AutoFixture;
using Events;
using EventStore;
using FluentAssertions;
using Framework;
using Resources;
using System;
using System.Threading.Tasks;
using Vereniging;
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

        var eventStoreMock = new EventStoreMock(
            (dynamic)verenigingWerdGeregistreerd,
            fixture.Create<VerenigingWerdVerwijderd>() with{ VCode = verenigingWerdGeregistreerd.VCode});

        var repo = new VerenigingsRepository(eventStoreMock);

        var exception = await
            Assert.ThrowsAsync<AssociationRegistry.Vereniging.Exceptions.VerenigingIsVerwijderd>(async () => await repo.Load<Vereniging>(VCode.Create(verenigingWerdGeregistreerd.VCode), expectedVersion: null)) ;

        exception.Message.Should().Be(ExceptionMessages.VerenigingIsVerwijderd);
    }
}
