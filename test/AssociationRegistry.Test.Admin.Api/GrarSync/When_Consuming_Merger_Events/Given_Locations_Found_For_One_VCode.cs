namespace AssociationRegistry.Test.Admin.Api.GrarSync.When_Consuming_Merger_Events;

using AssociationRegistry.Admin.Api.GrarSync;
using AssociationRegistry.Admin.Api.Infrastructure.AWS;
using AutoFixture;
using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;
using Common.AutoFixture;
using Grar.HeradresseerLocaties;
using Moq;
using Xunit;

public class Given_Locations_Found_For_One_VCode
{
    [Fact]
    public async Task Then_It_Sends_One_Queue_Message()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var message = fixture.Create<AddressWasRetiredBecauseOfMunicipalityMerger>();

        var sqsClientWrapperMock = new Mock<ISqsClientWrapper>();
        var teHeradresserenLocatiesFinder = new Mock<ITeHeradresserenLocatiesFinder>();

        teHeradresserenLocatiesFinder.Setup(s => s.Find(message.AddressPersistentLocalId))
                     .ReturnsAsync([fixture.Create<TeHeradresserenLocatiesMessage>()]);

        var sut = new AdresMergerHandler(sqsClientWrapperMock.Object, teHeradresserenLocatiesFinder.Object);

        await sut.Handle(message);

        sqsClientWrapperMock.Verify(v => v.QueueReaddressMessage(It.IsAny<TeHeradresserenLocatiesMessage>()), Times.Once());
    }
}
