namespace AssociationRegistry.Test.Admin.Api.GrarSync.When_Consuming_Merger_Events.TeHeradresserenLocatiesFinder;

using AssociationRegistry.Admin.Api.GrarConsumer;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Moq;
using Xunit;

public class Given_No_Locations_For_AdresId
{
    [Fact]
    public async Task Then_It_Returns_Empty_Array_Of_Messages()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var locatieFinder = new Mock<ILocatieFinder>();
        var adresId = fixture.Create<int>();

        locatieFinder.Setup(s => s.FindLocaties(adresId))
                     .ReturnsAsync([]);

        var sut = new TeHeradresserenLocatiesFinder(locatieFinder.Object);

        var actual = await sut.Find(adresId);

        actual.Should().BeEmpty();
    }
}
