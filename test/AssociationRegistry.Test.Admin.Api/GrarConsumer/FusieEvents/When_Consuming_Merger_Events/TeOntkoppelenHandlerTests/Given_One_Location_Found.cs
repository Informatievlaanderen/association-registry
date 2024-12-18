namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Consuming_Merger_Events.TeOntkoppelenHandlerTests;

using Acties.GrarConsumer.OntkoppelAdres;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Grar.GrarUpdates.Fusies.TeOntkoppelenLocaties;
using Grar.GrarUpdates.LocatieFinder;
using Moq;
using Xunit;

public class Given_One_Location_Found
{
    [Fact]
    public async Task Then_One_Message_Is_Sent()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var sourceAdresId = fixture.Create<int>();

        OntkoppelLocatiesMessage actual = null;
        var sqsClientWrapper = new Mock<ISqsClientWrapper>();
        sqsClientWrapper.CaptureQueueOverkoepelendeGrarMessage(message => actual = message.OntkoppelLocatiesMessage);

        var locatieId = fixture.Create<LocatieLookupData>();
        var locatiesFinder = new StubLocatieFinder(sourceAdresId, [locatieId]);
        var locatieIdsPerVCode = await locatiesFinder.FindLocaties(sourceAdresId);

        var sut = new TeOntkoppelenLocatiesProcessor(sqsClientWrapper.Object, locatiesFinder);
        await sut.Process(sourceAdresId);

        actual.Should().BeEquivalentTo(
            new OntkoppelLocatiesMessage
            (
                locatieIdsPerVCode.First().VCode,
                locatieIdsPerVCode.First().Locaties.Select(x => x.LocatieId).ToArray()
            ));
    }
}
