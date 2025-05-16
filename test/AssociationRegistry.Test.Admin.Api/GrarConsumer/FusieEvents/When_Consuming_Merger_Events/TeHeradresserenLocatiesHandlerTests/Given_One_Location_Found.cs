namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Consuming_Merger_Events.TeHeradresserenLocatiesHandlerTests;

using AssociationRegistry.Framework;
using AssociationRegistry.Grar.GrarConsumer.Messaging.HeradresseerLocaties;
using AssociationRegistry.Grar.GrarUpdates.Fusies.TeHeradresserenLocaties;
using AssociationRegistry.Grar.GrarUpdates.LocatieFinder;
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

        HeradresseerLocatiesMessage actual = null;
        var sqsClientWrapper = new Mock<ISqsClientWrapper>();
        sqsClientWrapper.CaptureQueueOverkoepelendeGrarMessage(message => actual = message.HeradresseerLocatiesMessage);

        var locatieId = fixture.Create<LocatieLookupData>();
        var idempotencyKey = fixture.Create<string>();
        var locatiesFinder = new StubLocatieFinder(sourceAdresId, [locatieId]);
        var locatieIdsPerVCode = await locatiesFinder.FindLocaties(sourceAdresId);

        var sut = new TeHeradresserenLocatiesProcessor(sqsClientWrapper.Object, locatiesFinder);
        await sut.Process(sourceAdresId, destinationAdresId, idempotencyKey);

        var messages = locatieIdsPerVCode.Map(destinationAdresId, idempotencyKey);

        actual.Should().BeEquivalentTo(messages.First());
    }
}
