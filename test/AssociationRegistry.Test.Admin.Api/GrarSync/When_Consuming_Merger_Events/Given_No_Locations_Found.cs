namespace AssociationRegistry.Test.Admin.Api.GrarSync.When_Consuming_Merger_Events;

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
        var locatieFinder = new Mock<ILocatieFinder>();

        locatieFinder.Setup(s => s.FindLocaties(message.AddressPersistentLocalId.ToString()))
                     .ReturnsAsync([]);

        var sut = new AdresMergerHandler(sqsClientWrapperMock.Object, locatieFinder.Object);

        await sut.Handle(message);

        sqsClientWrapperMock.Verify(v => v.QueueReaddressMessage(It.IsAny<TeHeradresserenLocatiesMessage>()), Times.Never());
    }
}

public class AdresMergerHandler
{
    private readonly ISqsClientWrapper _sqsClientWrapper;
    private readonly ILocatieFinder _locatieFinder;

    public AdresMergerHandler(ISqsClientWrapper sqsClientWrapper, ILocatieFinder locatieFinder)
    {
        _sqsClientWrapper = sqsClientWrapper;
        _locatieFinder = locatieFinder;
    }

    public Task Handle(AddressWasRetiredBecauseOfMunicipalityMerger adres)
    {
        return Task.CompletedTask;
    }

}
