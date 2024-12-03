namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Consuming_Merger_Events.TeHeradresserenLocatiesHandlerTests;

using Acties.HeradresseerLocaties;
using AssociationRegistry.Admin.Api.Infrastructure.AWS;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Grar.GrarUpdates.Fusies.TeHeradresserenLocaties;
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
        var destinationAdresId = fixture.Create<int>();

        HeradresseerLocatiesMessage actual = null;
        var sqsClientWrapper = new Mock<ISqsClientWrapper>();
        sqsClientWrapper.CaptureQueueReaddressMessage(message => actual = message);

        var locatieId = fixture.Create<LocatieLookupData>();
        var locatiesFinder = new StubLocatieFinder(sourceAdresId, [locatieId]);
        var locatieIdsPerVCode = await locatiesFinder.FindLocaties(sourceAdresId);

        var sut = new TeHeradresserenLocatiesProcessor(sqsClientWrapper.Object, locatiesFinder);
        await sut.Process(sourceAdresId, destinationAdresId);

        var messages = locatieIdsPerVCode.Map(destinationAdresId);

        actual.Should().BeEquivalentTo(messages.First());
    }
}
