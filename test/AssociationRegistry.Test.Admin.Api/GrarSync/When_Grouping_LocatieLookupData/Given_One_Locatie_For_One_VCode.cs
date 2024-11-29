namespace AssociationRegistry.Test.Admin.Api.GrarSync.When_Grouping_LocatieLookupData;

using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Admin.Api.GrarConsumer.Groupers;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Grar.GrarConsumer.TeHeradresserenLocaties;
using Grar.Models;
using Vereniging;
using Xunit;

public class Given_One_Locatie_For_One_VCode
{
    [Fact]
    public void Then_Returns_One_TeHeradresserenLocatiesMessage_For_VCode()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var destinationAdresId = fixture.Create<int>();

        var lookupData = new LocatieLookupData(fixture.Create<VCode>(), fixture.Create<int>(), fixture.Create<string>());

        var actual = LocatiesVolgensVCodeGrouper.Group([lookupData], destinationAdresId);

        actual.Should().BeEquivalentTo([
            new TeHeradresserenLocatiesMessage(
                lookupData.VCode,
                [
                    new TeHeradresserenLocatie(lookupData.LocatieId,
                                               destinationAdresId.ToString()),
                ],
                ""),
        ]);
    }
}
