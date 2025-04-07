namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Consuming_Merger_Events.AdresMergerHandler;

using AssociationRegistry.Grar.GrarUpdates.Fusies;
using AssociationRegistry.Grar.GrarUpdates.Fusies.TeHeradresserenLocaties;
using AssociationRegistry.Grar.GrarUpdates.Fusies.TeOntkoppelenLocaties;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using Moq;
using Xunit;

public class Given_A_Destination_Adres
{
    [Fact]
    public async Task Then_It_Calls_TeHeradresserenLocatiesHandler()
    {
        var teHeradresserenLocatiesHandler = new Mock<ITeHeradresserenLocatiesProcessor>();
        var teOntkoppelenLocatiesHandler = new Mock<ITeOntkoppelenLocatiesProcessor>();

        var sut = new FusieEventProcessor(teHeradresserenLocatiesHandler.Object, teOntkoppelenLocatiesHandler.Object);

        var fixture = new Fixture().CustomizeAdminApi();
        var sourceAdresId = fixture.Create<int>();
        var destinationAdresId = fixture.Create<int>();
        var idempotencyKey = fixture.Create<string>();
        await sut.Process(sourceAdresId, destinationAdresId, idempotencyKey);

        teHeradresserenLocatiesHandler.Verify(v => v.Process(sourceAdresId, destinationAdresId, idempotencyKey), Times.Once());
        teOntkoppelenLocatiesHandler.Verify(v => v.Process(It.IsAny<int>()), Times.Never());
    }
}
