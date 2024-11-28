namespace AssociationRegistry.Test.Admin.Api.GrarSync.When_Consuming_Merger_Events.AdresMergerHandler;

using AssociationRegistry.Admin.Api.GrarSync;
using AssociationRegistry.Admin.Api.Infrastructure.AWS;
using AssociationRegistry.Grar.HeradresseerLocaties;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;
using Moq;
using Xunit;

public class Given_Locations_Found_For_Multiple_VCodes
{
    [Fact]
    public async Task Then_It_Sends_Multiple_Messages_On_The_Queue()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var message = fixture.Create<AddressWasRetiredBecauseOfMunicipalityMerger>();

        var sqsClientWrapperMock = new Mock<ISqsClientWrapper>();
        var teHeradresserenLocatiesFinder = new Mock<ITeHeradresserenLocatiesFinder>();

        var teHeradresserenLocatiesMessages = fixture.CreateMany<TeHeradresserenLocatiesMessage>();

        teHeradresserenLocatiesFinder.Setup(s => s.Find(message.AddressPersistentLocalId))
                                     .ReturnsAsync(teHeradresserenLocatiesMessages.ToArray);

        var sut = new AdresMergerHandler(sqsClientWrapperMock.Object, teHeradresserenLocatiesFinder.Object);

        await sut.Handle(message);

        sqsClientWrapperMock.Verify(v => v.QueueReaddressMessage(
                                        It.IsAny<TeHeradresserenLocatiesMessage>()),
                                    Times.Exactly(teHeradresserenLocatiesMessages.Count()));
    }
}
