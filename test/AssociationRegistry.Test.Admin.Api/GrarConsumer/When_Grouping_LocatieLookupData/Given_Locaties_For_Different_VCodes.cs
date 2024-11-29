namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.When_Grouping_LocatieLookupData;

using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Admin.Api.GrarConsumer.Groupers;
using AssociationRegistry.Grar.GrarConsumer.TeHeradresserenLocaties;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_Locaties_For_Different_VCodes
{
    [Fact]
    public void Then_Returns_One_TeHeradresserenLocatiesMessage_For_VCode()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var destinationAdresId = fixture.Create<int>();
        var stringDestinationAdresId = destinationAdresId.ToString();
        var sourceAdresId = fixture.Create<int>().ToString();

        var vCode1 = fixture.Create<string>();
        var vCode2 = fixture.Create<string>();

        var locatieLookupData = new[]
        {
            fixture.Create<LocatieLookupData>() with
            {
                VCode = vCode1,
                AdresId = sourceAdresId,
                LocatieId = 1
            },
            fixture.Create<LocatieLookupData>() with
            {
                VCode = vCode2,
                AdresId = sourceAdresId,
                LocatieId = 1
            },
            fixture.Create<LocatieLookupData>() with
            {
                VCode = vCode1,
                AdresId = sourceAdresId,
                LocatieId = 2
            },
        };

        var actual = LocatiesVolgensVCodeGrouper.Group(locatieLookupData, destinationAdresId);

        actual.Should().BeEquivalentTo(
            [
                new TeHeradresserenLocatiesMessage(
                    vCode1,
                    [
                        new(1, stringDestinationAdresId),
                        new(2, stringDestinationAdresId),
                    ]
                   ,
                    ""),
                new TeHeradresserenLocatiesMessage(
                    vCode2,
                    [
                        new(1, stringDestinationAdresId),
                    ],
                    ""),
            ]);
    }
}
