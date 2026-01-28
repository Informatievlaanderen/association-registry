namespace AssociationRegistry.Test.AggregateSessionTests.When_Loading_A_Vereniging;

using AutoFixture;
using AutoFixture.Kernel;
using Common.AutoFixture;
using Common.Framework;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using Events;
using EventStore;
using FluentAssertions;
using Framework;
using MartenDb.Store;
using Resources;
using Vereniging;
using Xunit;

public class Given_A_Dubbele_Vereniging
{
    [Theory]
    [InlineData(typeof(FeitelijkeVerenigingWerdGeregistreerd))]
    [InlineData(typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd))]
    public async Task Then_Throws_A_VerenigingIsDubbelException(Type verenigingType)
    {
        var fixture = new Fixture().CustomizeDomain();
        var context = new SpecimenContext(fixture);

        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)
            context.Resolve(verenigingType);

        var eventStoreMock = new EventStoreMock(
            (dynamic)verenigingWerdGeregistreerd,
            fixture.Create<VerenigingWerdGemarkeerdAlsDubbelVan>() with
            {
                VCode = verenigingWerdGeregistreerd.VCode,
                VCodeAuthentiekeVereniging = fixture.Create<VCode>(),
            }
        );

        var repo = new AggregateSession(eventStoreMock);

        var exception = await Assert.ThrowsAsync<VerenigingIsDubbel>(async () =>
            await repo.Load<Vereniging>(
                VCode.Create(verenigingWerdGeregistreerd.VCode),
                metadata: TestCommandMetadata.Empty
            )
        );

        exception.Message.Should().Be(ExceptionMessages.VerenigingIsDubbel);
    }
}
