namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.AdresMergerEvents.When_Consuming_Merger_Events.TeHeradresserenLocatiesHandlerTests;

using AssociationRegistry.Admin.Api.GrarConsumer.Handlers;
using AssociationRegistry.Admin.Api.Infrastructure.AWS;
using AssociationRegistry.Grar.GrarUpdates.TeHeradresserenLocaties;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Moq;
using Xunit;

public class Given_One_Location_Found
{
    [Fact]
    public async Task Then_One_Message_Is_Sent()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var sourceAdresId = fixture.Create<int>();
        var destinationAdresId = fixture.Create<int>();

        TeHeradresserenLocatiesMessage actual = null;
        var sqsClientWrapper = new Mock<ISqsClientWrapper>();
        sqsClientWrapper.CaptureQueueReaddressMessage(message => actual = message);

        var locatieId = fixture.Create<int>();
        var locatiesFinder = new StubLocatieFinder(sourceAdresId, [locatieId]);
        var locatieIdsPerVCode = await locatiesFinder.FindLocaties([locatieId]);

        var sut = new TeHeradresserenLocatiesHandler(sqsClientWrapper.Object, locatiesFinder);
        await sut.Handle(sourceAdresId, destinationAdresId);

        var messages = locatieIdsPerVCode.Map(destinationAdresId);

        actual.Should().BeEquivalentTo(messages.First());
    }
}
