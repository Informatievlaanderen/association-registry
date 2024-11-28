namespace AssociationRegistry.Test.Admin.Api.GrarSync.When_Consuming_Merger_Events.TeHeradresserenLocatiesFinder;

using AssociationRegistry.Admin.Api.GrarSync;
using AssociationRegistry.Admin.Api.ProjectieBeheer.ResponseModels;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Grar.HeradresseerLocaties;
using Grar.Models;
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

public class Given_One_Location_For_AdresId
{
    [Fact]
    public async Task Then_It_Returns_Grouped_Messages()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var locatieFinder = new Mock<ILocatieFinder>();
        var adresId = fixture.Create<int>();
        var locatiesVolgensVCode = fixture.Create<LocatiesVolgensVCode>();

        locatieFinder.Setup(s => s.FindLocaties(adresId))
                     .ReturnsAsync([locatiesVolgensVCode]);

        var sut = new TeHeradresserenLocatiesFinder(locatieFinder.Object);

        var actual = await sut.Find(adresId);

        actual.Should().NotBeEmpty();
        actual.Count().Should().Be(1);

        actual.First().Should().BeEquivalentTo(
            new TeHeradresserenLocatiesMessage(
                locatiesVolgensVCode.VCode,
                locatiesVolgensVCode.LocatiesMetAdresId.Select(s => new LocatieIdWithAdresId(s.LocatieId, s.AddressId)).ToList(),
                ""), config: options => options.Excluding(x => x.idempotencyKey));
    }
}

public class TeHeradresserenLocatiesFinder : ITeHeradresserenLocatiesFinder
{
    private readonly ILocatieFinder _locatieFinder;

    public TeHeradresserenLocatiesFinder(ILocatieFinder locatieFinder)
    {
        _locatieFinder = locatieFinder;
    }

    public async Task<TeHeradresserenLocatiesMessage[]> Find(int addressPersistentLocalId)
    {
        var locatiesMetVCodes = await _locatieFinder.FindLocaties(addressPersistentLocalId);

        return !locatiesMetVCodes.Any()
            ? []
            : new LocatiesVolgensVCodeGrouper().Group(locatiesMetVCodes);
    }
}

public class LocatiesVolgensVCodeGrouper
{
    public TeHeradresserenLocatiesMessage[] Group(LocatiesVolgensVCode[] locatiesVolgensVCodes)
    {
        return Array.Empty<TeHeradresserenLocatiesMessage>();
    }
}
