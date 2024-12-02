﻿namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Consuming_Merger_Events.TeHeradresserenLocatiesHandlerTests;

using AssociationRegistry.Admin.Api.GrarConsumer.Handlers.Fusies;
using AssociationRegistry.Admin.Api.Infrastructure.AWS;
using AutoFixture;
using Common.AutoFixture;
using Grar.GrarUpdates.TeHeradresserenLocaties;
using Grar.LocatieFinder;
using Moq;
using Xunit;

public class Given_No_Locations_Found
{
    [Fact]
    public async Task Then_No_Messages_Are_Sent()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var sourceAdresId = fixture.Create<int>();
        var destinationAdresId = fixture.Create<int>();

        var sqsClientWrapperMock = new Mock<ISqsClientWrapper>();
        var locatiesFinder = new Mock<ILocatieFinder>();

        locatiesFinder.Setup(s => s.FindLocaties(sourceAdresId))
                                     .ReturnsAsync(LocatieIdsPerVCodeCollection.Empty);

        var sut = new TeHeradresserenLocatiesHandler(sqsClientWrapperMock.Object, locatiesFinder.Object);
        await sut.Handle(sourceAdresId, destinationAdresId);

        sqsClientWrapperMock.Verify(v => v.QueueReaddressMessage(It.IsAny<TeHeradresserenLocatiesMessage>()), Times.Never());
    }
}
