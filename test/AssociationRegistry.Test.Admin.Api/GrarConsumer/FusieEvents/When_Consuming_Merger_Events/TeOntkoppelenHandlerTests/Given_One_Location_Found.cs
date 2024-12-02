namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Consuming_Merger_Events.TeOntkoppelenHandlerTests;

using AssociationRegistry.Admin.Api.GrarConsumer.Handlers.Fusies;
using AssociationRegistry.Admin.Api.Infrastructure.AWS;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Grar.GrarUpdates.TeOnkoppelenLocaties;
using Moq;
using Xunit;

public class Given_One_Location_Found
{
    [Fact]
    public async Task Then_One_Message_Is_Sent()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var sourceAdresId = fixture.Create<int>();

        TeOntkoppelenLocatiesMessage actual = null;
        var sqsClientWrapper = new Mock<ISqsClientWrapper>();
        sqsClientWrapper.CaptureQueueOntkoppelMessage(message => actual = message);

        var locatieId = fixture.Create<int>();
        var locatiesFinder = new StubLocatieFinder(sourceAdresId, [locatieId]);
        var locatieIdsPerVCode = await locatiesFinder.FindLocaties(sourceAdresId);

        var sut = new TeOntkoppelenLocatieHandler(sqsClientWrapper.Object, locatiesFinder);
        await sut.Handle(sourceAdresId);

        actual.Should().BeEquivalentTo(
            new TeOntkoppelenLocatiesMessage
            (
                locatieIdsPerVCode.First().VCode,
                locatieIdsPerVCode.First().LocatieIds
            ));
    }
}
