namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Consuming_Merger_Events.AdresMergerHandler;

using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using Grar.GrarUpdates.Fusies;
using Moq;
using Xunit;

public class Given_No_Destination_Adres
{
    [Fact]
    public async Task Then_It_Calls_TeOntkoppelenHandler()
    {
        var teHeradresserenLocatiesHandler = new Mock<ITeHeradresserenLocatiesHandler>();
        var teOntkoppelenLocatiesHandler = new Mock<ITeOntkoppelenLocatiesHandler>();

        var sut = new FusieEventHandler(teHeradresserenLocatiesHandler.Object, teOntkoppelenLocatiesHandler.Object);

        var fixture = new Fixture().CustomizeAdminApi();
        var sourceAdresId = fixture.Create<int>();
        await sut.Handle(sourceAdresId, null);

        teHeradresserenLocatiesHandler.Verify(v => v.Handle(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        teOntkoppelenLocatiesHandler.Verify(v => v.Handle(sourceAdresId), Times.Once());
    }
}
