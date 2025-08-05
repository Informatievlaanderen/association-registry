namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Consuming_Merger_Events.TeOntkoppelenHandlerTests;

using AssociationRegistry.Framework;
using AssociationRegistry.Grar.GrarConsumer.Messaging.OntkoppelAdres;
using AssociationRegistry.Grar.GrarUpdates.Fusies.TeOntkoppelenLocaties;
using AssociationRegistry.Grar.GrarUpdates.LocatieFinder;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Moq;
using Wolverine;
using Xunit;

public class Given_One_Location_Found
{
    [Fact]
    public async ValueTask Then_One_Message_Is_Sent()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var sourceAdresId = fixture.Create<int>();

        OntkoppelLocatiesMessage actual = null;
        var messageBusMock = new Mock<IMessageBus>();
        messageBusMock.CaptureQueueOverkoepelendeGrarMessage(message => actual = message.OntkoppelLocatiesMessage);

        var locatieId = fixture.Create<LocatieLookupData>();
        var locatiesFinder = new StubLocatieFinder(sourceAdresId, [locatieId]);
        var locatieIdsPerVCode = await locatiesFinder.FindLocaties(sourceAdresId);

        var sut = new TeOntkoppelenLocatiesProcessor(messageBusMock.Object, locatiesFinder);
        await sut.Process(sourceAdresId);

        actual.Should().BeEquivalentTo(
            new OntkoppelLocatiesMessage
            (
                locatieIdsPerVCode.First().VCode,
                locatieIdsPerVCode.First().Locaties.Select(x => x.LocatieId).ToArray()
            ));
    }
}
