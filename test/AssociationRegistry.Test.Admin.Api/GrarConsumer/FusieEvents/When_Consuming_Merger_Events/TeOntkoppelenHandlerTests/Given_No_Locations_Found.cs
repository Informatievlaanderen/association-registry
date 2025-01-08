﻿namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Consuming_Merger_Events.TeOntkoppelenHandlerTests;

using Acties.GrarConsumer.OntkoppelAdres;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Grar.GrarUpdates.Fusies.TeOntkoppelenLocaties;
using Grar.GrarUpdates.LocatieFinder;
using Moq;
using Xunit;

public class Given_No_Locations_Found
{
    [Fact]
    public async Task Then_No_Messages_Are_Sent()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var sourceAdresId = fixture.Create<int>();

        var sqsClientWrapperMock = new Mock<ISqsClientWrapper>();
        var locatiesFinder = new Mock<ILocatieFinder>();

        locatiesFinder.Setup(s => s.FindLocaties(sourceAdresId))
                      .ReturnsAsync(LocatiesPerVCodeCollection.Empty);

        var sut = new TeOntkoppelenLocatiesProcessor(sqsClientWrapperMock.Object, locatiesFinder.Object);
        await sut.Process(sourceAdresId);

        sqsClientWrapperMock.Verify(v => v.QueueMessage(It.IsAny<OntkoppelLocatiesMessage>()), Times.Never());
    }
}