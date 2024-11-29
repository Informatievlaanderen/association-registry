namespace AssociationRegistry.Test.Admin.Api.GrarSync.When_Consuming_Merger_Events.TeHeradresserenLocatiesHandlerTests;

using AssociationRegistry.Admin.Api.GrarConsumer;
using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Admin.Api.GrarConsumer.Groupers;
using AssociationRegistry.Admin.Api.GrarConsumer.Handlers;
using AssociationRegistry.Admin.Api.Infrastructure.AWS;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Grar.GrarConsumer.TeHeradresserenLocaties;
using Moq;
using Xunit;

public class Given_One_Location_Found
{
    [Fact]
    public async Task Then_One_Message_Send()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var sourceAdresId = fixture.Create<int>();
        var destinationAdresId = fixture.Create<int>();

        TeHeradresserenLocatiesMessage actual = null;
        var sqsClientWrapper = new Mock<ISqsClientWrapper>();
        sqsClientWrapper.CaptureQueueReaddressMessage(message => actual = message);

        LocatieLookupData[] locatieLookupData = [fixture.Create<LocatieLookupData>()];
        var locatiesFinder = new StubLocatieFinder(sourceAdresId, locatieLookupData);

        var sut = new TeHeradresserenLocatiesHandler(sqsClientWrapper.Object, locatiesFinder);
        await sut.Handle(sourceAdresId, destinationAdresId);

        var messages = LocatiesVolgensVCodeGrouper.Group(locatieLookupData, destinationAdresId);
        actual.Should().BeEquivalentTo(messages.First());
    }
}
