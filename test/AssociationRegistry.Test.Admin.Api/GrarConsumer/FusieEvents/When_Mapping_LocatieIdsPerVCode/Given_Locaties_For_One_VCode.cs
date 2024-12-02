namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Mapping_LocatieIdsPerVCode;

using Acties.HeradresseerLocaties;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Grar.GrarUpdates.Fusies.TeHeradresserenLocaties;
using Grar.GrarUpdates.Hernummering;
using Grar.GrarUpdates.LocatieFinder;
using Xunit;

public class Given_Locaties_For_One_VCode
{
    [Fact]
    public void Then_Returns_One_HeradresseerLocatiesMessage_For_VCode()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var destinationAdresId = fixture.Create<int>();

        var vCode = fixture.Create<string>();

        var locatieIds = fixture.Create<LocatieLookupData[]>();

        var locatieIdsPerVCode = LocatiesPerVCodeCollection.FromLocatiesPerVCode(new Dictionary<string, LocatieLookupData[]>()
        {
            {vCode, locatieIds},
        });

        var actual = locatieIdsPerVCode.Map(destinationAdresId);

        actual.Should().BeEquivalentTo([
            new HeradresseerLocatiesMessage(
                vCode,
                locatieIds.Select(l => new TeHeradresserenLocatie(l.LocatieId, destinationAdresId.ToString())).ToList(),
                ""),
        ]);
    }
}
