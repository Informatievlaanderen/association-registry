namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Mapping_LocatieIdsPerVCode;

using AssociationRegistry.Grar.GrarUpdates.TeHeradresserenLocaties;
using AssociationRegistry.Grar.LocatieFinder;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_Locaties_For_One_VCode
{
    [Fact]
    public void Then_Returns_One_TeHeradresserenLocatiesMessage_For_VCode()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var destinationAdresId = fixture.Create<int>();

        var vCode = fixture.Create<string>();

        var locatieIds = fixture.Create<int[]>();

        var locatieIdsPerVCode = LocatieIdsPerVCodeCollection.FromLocatiesPerVCode(new Dictionary<string, int[]>()
        {
            {vCode, locatieIds},
        });

        var actual = locatieIdsPerVCode.Map(destinationAdresId);

        actual.Should().BeEquivalentTo([
            new TeHeradresserenLocatiesMessage(
                vCode,
                locatieIds.Select(l => new TeHeradresserenLocatie(l, destinationAdresId.ToString())).ToList(),
                ""),
        ]);
    }
}
