namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.AdresMergerEvents.When_Mapping_LocatieIdsPerVCode;

using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Admin.Api.GrarConsumer.Handlers;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Grar.GrarUpdates.TeHeradresserenLocaties;
using Grar.Models;
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

        var locatieIdsPerVCode = new LocatieIdsPerVCodeCollection(new Dictionary<string, int[]>()
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
