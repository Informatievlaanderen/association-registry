namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Mapping_LocatieIdsPerVCode;

using AssociationRegistry.Grar.Models;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Grar.GrarUpdates.Fusies.TeHeradresserenLocaties;
using Grar.GrarUpdates.LocatieFinder;
using Xunit;

public class Given_Locaties_For_Different_VCodes
{
    [Fact]
    public void Then_Returns_TeHeradresserenLocatiesMessage_For_VCode()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var destinationAdresId = fixture.Create<int>();

        var vCode1 = fixture.Create<string>();
        var locatieIdsForVCode1 = fixture.Create<int[]>();

        var vCode2 = fixture.Create<string>();
        var locatieIdsForVCode2 = fixture.Create<int[]>();

        var locatieIdsPerVCode = LocatieIdsPerVCodeCollection.FromLocatiesPerVCode(new Dictionary<string, int[]>()
        {
            { vCode1, locatieIdsForVCode1 },
            { vCode2, locatieIdsForVCode2 },
        });

        var actual = locatieIdsPerVCode.Map(destinationAdresId);

        actual.Should().BeEquivalentTo([
            new TeHeradresserenLocatiesMessage(
                vCode1,
                locatieIdsForVCode1.Select(l => new TeHeradresserenLocatie(l, destinationAdresId.ToString())).ToList(),
                ""),
            new TeHeradresserenLocatiesMessage(
                vCode2,
                locatieIdsForVCode2.Select(l => new TeHeradresserenLocatie(l, destinationAdresId.ToString())).ToList(),
                ""),
        ]);
    }
}
