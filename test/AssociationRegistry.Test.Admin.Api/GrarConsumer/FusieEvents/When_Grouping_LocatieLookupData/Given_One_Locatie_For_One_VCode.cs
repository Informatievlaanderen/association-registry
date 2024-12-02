namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Grouping_LocatieLookupData;

using Acties.HeradresseerLocaties;
using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Admin.Api.GrarConsumer.Handlers.StraatHernummering.Groupers;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Grar.GrarUpdates.Fusies.TeHeradresserenLocaties;
using Xunit;

public class Given_One_Locatie_For_One_VCode
{
    [Fact]
    public void Then_Returns_One_HeradresseerLocatiesMessage_For_VCode()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var destinationAdresId = fixture.Create<int>();

        var lookupData = new LocatieMetVCode(fixture.Create<VCode>(), fixture.Create<int>());

        var actual = LocatiesVolgensVCodeGrouper.Group([lookupData], destinationAdresId);

        actual.Should().BeEquivalentTo([
            new HeradresseerLocatiesMessage(
                lookupData.VCode,
                [
                    new TeHeradresserenLocatie(lookupData.LocatieId,
                                               destinationAdresId.ToString()),
                ],
                ""),
        ]);
    }
}
