namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.AdresMergerEvents.When_Grouping_LocatieLookupData;

using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Admin.Api.GrarConsumer.Groupers;
using AssociationRegistry.Grar.GrarUpdates.TeHeradresserenLocaties;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_One_Locatie_For_One_VCode
{
    [Fact]
    public void Then_Returns_One_TeHeradresserenLocatiesMessage_For_VCode()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var destinationAdresId = fixture.Create<int>();

        var lookupData = new LocatieMetVCode(fixture.Create<VCode>(), fixture.Create<int>());

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
