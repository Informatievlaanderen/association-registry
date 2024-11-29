namespace AssociationRegistry.Test.Admin.Api.GrarSync.When_Consuming_Merger_Events.AdresMergerHandler;

using AssociationRegistry.Admin.Api.GrarConsumer;
using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Admin.Api.Infrastructure.AWS;
using Grar.HeradresseerLocaties;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;
using Moq;
using Xunit;

public class Given_Locations_Found_For_One_VCode
{
    [Fact]
    public async Task Then_It_Sends_One_Message_On_The_Queue()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var adresId = fixture.Create<int>();

        var sqsClientWrapperMock = new Mock<ISqsClientWrapper>();
        var teHeradresserenLocatiesFinder = new Mock<ITeHeradresserenLocatiesFinder>();

        teHeradresserenLocatiesFinder.Setup(s => s.Find(adresId))
                     .ReturnsAsync([fixture.Create<TeHeradresserenLocatiesMessage>()]);

        var sut = new AdresMergerHandler(sqsClientWrapperMock.Object, teHeradresserenLocatiesFinder.Object);

        await sut.Handle(adresId);

        sqsClientWrapperMock.Verify(v => v.QueueReaddressMessage(It.IsAny<TeHeradresserenLocatiesMessage>()), Times.Once());
    }
}
