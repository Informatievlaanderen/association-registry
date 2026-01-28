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
using Vereniging;
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

        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)
            context.Resolve(verenigingType);

        var eventStoreMock = new EventStoreMock((dynamic)verenigingWerdGeregistreerd);

        var repo = new AggregateSession(eventStoreMock);

        var loadMethod = () =>
            repo.Load<VerenigingMetRechtspersoonlijkheid>(
                VCode.Create(verenigingWerdGeregistreerd.VCode),
                metadata: TestCommandMetadata.Empty
            );

        await loadMethod.Should().ThrowAsync<ActieIsNietToegestaanVoorVerenigingstype>();
    }
}
