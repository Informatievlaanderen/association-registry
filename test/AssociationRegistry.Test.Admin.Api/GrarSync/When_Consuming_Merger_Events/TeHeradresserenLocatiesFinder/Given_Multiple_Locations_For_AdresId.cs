namespace AssociationRegistry.Test.Admin.Api.GrarSync.When_Consuming_Merger_Events.TeHeradresserenLocatiesFinder;

using AssociationRegistry.Admin.Api.GrarConsumer;
using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Grar.HeradresseerLocaties;
using Grar.Models;
using Moq;
using Xunit;

public class Given_Multiple_Locations_For_AdresId
{
    [Fact]
    public async Task Then_It_Returns_Grouped_Messages()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var locatieFinder = new Mock<ILocatieFinder>();
        var adresId = fixture.Create<int>().ToString();

        var vCode1 = fixture.Create<string>();
        var vCode2 = fixture.Create<string>();

        var locatiesVolgensVCode = new[]
        {
            fixture.Create<LocatieLookupData>() with
            {
                VCode = vCode1,
                AdresId = adresId,
                LocatieId = 1
            },
            fixture.Create<LocatieLookupData>() with
            {
                VCode = vCode1,
                AdresId = adresId,
                LocatieId = 2
            },
            fixture.Create<LocatieLookupData>() with
            {
                VCode = vCode2,
                AdresId = adresId,
                LocatieId = 1
            },
            fixture.Create<LocatieLookupData>() with
            {
                VCode = vCode1,
                AdresId = adresId,
                LocatieId = 3
            },
            fixture.Create<LocatieLookupData>() with
            {
                VCode = vCode2,
                AdresId = adresId,
                LocatieId = 2
            },
        };

        locatieFinder.Setup(s => s.FindLocaties(Convert.ToInt32(adresId)))
                     .ReturnsAsync(locatiesVolgensVCode);

        var sut = new TeHeradresserenLocatiesFinder(locatieFinder.Object);
        var actual = await sut.Find(Convert.ToInt32(adresId));

        actual.Should().NotBeEmpty();
        actual.Count().Should().Be(locatiesVolgensVCode.Select(x => x.VCode).Distinct().Count());

        actual.Should()
              .BeEquivalentTo([
                                  new TeHeradresserenLocatiesMessage(
                                      vCode1,
                                      new LocatieIdWithAdresId[] { new(1, adresId), new(2, adresId), new(3, adresId) }.ToList(),
                                      ""),
                                  new TeHeradresserenLocatiesMessage(
                                      vCode2,
                                      new LocatieIdWithAdresId[] { new(1, adresId), new(2, adresId) }.ToList(),
                                      ""),
                              ],
                              config: options => options.Excluding(x => x.idempotencyKey));
    }
}
