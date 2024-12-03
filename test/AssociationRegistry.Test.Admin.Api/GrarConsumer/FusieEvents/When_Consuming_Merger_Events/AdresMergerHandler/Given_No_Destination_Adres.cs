namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Consuming_Merger_Events.AdresMergerHandler;

using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using Grar.GrarUpdates.Fusies;
using Grar.GrarUpdates.Fusies.TeHeradresserenLocaties;
using Grar.GrarUpdates.Fusies.TeOntkoppelenLocaties;
using Moq;
using Xunit;

public class Given_No_Destination_Adres
{
    [Fact]
    public async Task Then_It_Calls_TeOntkoppelenHandler()
    {
        var teHeradresserenLocatiesHandler = new Mock<ITeHeradresserenLocatiesProcessor>();
        var teOntkoppelenLocatiesHandler = new Mock<ITeOntkoppelenLocatiesProcessor>();

        var sut = new FusieEventProcessor(teHeradresserenLocatiesHandler.Object, teOntkoppelenLocatiesHandler.Object);

        var fixture = new Fixture().CustomizeAdminApi();
        var sourceAdresId = fixture.Create<int>();
        await sut.Process(sourceAdresId, null);

        teHeradresserenLocatiesHandler.Verify(v => v.Process(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        teOntkoppelenLocatiesHandler.Verify(v => v.Process(sourceAdresId), Times.Once());
    }
}
