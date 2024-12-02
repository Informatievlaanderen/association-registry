namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.AdresMergerEvents.When_Consuming_Merger_Events.AdresMergerHandler;

using AssociationRegistry.Admin.Api.GrarConsumer.Handlers;
using AssociationRegistry.Admin.Api.GrarConsumer.Handlers.Fusies;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using Moq;
using Xunit;

public class Given_A_Destination_Adres
{
    [Fact]
    public async Task Then_It_Calls_TeHeradresserenLocatiesHandler()
    {
        var teHeradresserenLocatiesHandler = new Mock<ITeHeradresserenLocatiesHandler>();
        var teOntkoppelenLocatiesHandler = new Mock<ITeOntkoppelenLocatiesHandler>();

        var sut = new AdresMergerHandler(teHeradresserenLocatiesHandler.Object, teOntkoppelenLocatiesHandler.Object);

        var fixture = new Fixture().CustomizeAdminApi();
        var sourceAdresId = fixture.Create<int>();
        var destinationAdresId = fixture.Create<int>();
        await sut.Handle(sourceAdresId, destinationAdresId);

        teHeradresserenLocatiesHandler.Verify(v => v.Handle(sourceAdresId, destinationAdresId), Times.Once());
        teOntkoppelenLocatiesHandler.Verify(v => v.Handle(It.IsAny<int>()), Times.Never());
    }
}
