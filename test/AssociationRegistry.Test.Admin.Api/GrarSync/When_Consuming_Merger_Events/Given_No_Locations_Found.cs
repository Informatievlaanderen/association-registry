﻿namespace AssociationRegistry.Test.Admin.Api.GrarSync.When_Consuming_Merger_Events;

using AssociationRegistry.Admin.Api.GrarSync;
using AssociationRegistry.Admin.Api.Infrastructure.AWS;
using AutoFixture;
using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;
using Common.AutoFixture;
using Grar.HeradresseerLocaties;
using Moq;
using Xunit;

public class Given_No_Locations_Found
{

    [Fact]
    public async Task Then_It_Does_Not_Queue_Anything()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var message = fixture.Create<AddressWasRetiredBecauseOfMunicipalityMerger>();

        var sqsClientWrapperMock = new Mock<ISqsClientWrapper>();
        var teHeradresserenLocatiesFinder = new Mock<ITeHeradresserenLocatiesFinder>();

        teHeradresserenLocatiesFinder.Setup(s => s.Find(message.AddressPersistentLocalId))
                                     .ReturnsAsync([]);

        var sut = new AdresMergerHandler(sqsClientWrapperMock.Object, teHeradresserenLocatiesFinder.Object);

        await sut.Handle(message);

        sqsClientWrapperMock.Verify(v => v.QueueReaddressMessage(It.IsAny<TeHeradresserenLocatiesMessage>()), Times.Never());
    }
}
