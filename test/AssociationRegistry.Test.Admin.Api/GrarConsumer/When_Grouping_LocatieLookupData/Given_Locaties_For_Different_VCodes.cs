namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.When_Grouping_LocatieLookupData;

using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Admin.Api.GrarConsumer.Groupers;
using AssociationRegistry.Grar.GrarConsumer.TeHeradresserenLocaties;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using Be.Vlaanderen.Basisregisters.Utilities;
using FluentAssertions;
using Grar.Models;
using JasperFx.Core;
using Xunit;

public class Given_Locaties_For_Different_VCodes
{
    [Fact]
    public void Then_Returns_One_TeHeradresserenLocatiesMessage_For_VCode()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var destinationAdresId = fixture.Create<int>();

        var locatieLookupData = new LocatieLookupTestData(fixture, fixture.Create<string>());

        var actual = LocatiesVolgensVCodeGrouper.Group(locatieLookupData, destinationAdresId);

        actual.Should().BeEquivalentTo(
        [
            new TeHeradresserenLocatiesMessage(
                locatieLookupData.VCode1,
                locatieLookupData.For(locatieLookupData.VCode1)
                                 .Select(x => new TeHeradresserenLocatie(x.LocatieId, destinationAdresId.ToString()))
                                 .ToList(),
                ""),
            new TeHeradresserenLocatiesMessage(
                locatieLookupData.VCode2,
                locatieLookupData.For(locatieLookupData.VCode2)
                                 .Select(x => new TeHeradresserenLocatie(x.LocatieId, destinationAdresId.ToString()))
                                 .ToList(),
                ""),
        ]);
    }

    public class LocatieLookupTestData : List<LocatieLookupData>
    {
        public string VCode1 => "1";
        public string VCode2 => "2";

        public LocatieLookupTestData For(string vCode)
        {
            return new(this.Where(x => x.VCode == vCode)
                           .ToArray());
        }

        public TeHeradresserenLocatie[] MapLocatieData(Func<LocatieLookupData, TeHeradresserenLocatie> map)
            => this.Select(map).ToArray();

        private LocatieLookupTestData(IEnumerable<LocatieLookupData> data) : base(data)
        {
        }

        public LocatieLookupTestData(IFixture fixture, string sourceAdresId)
        {
            AddRange(
            [
                fixture.Create<LocatieLookupData>() with
                {
                    VCode = VCode1,
                    AdresId = sourceAdresId,
                    LocatieId = 1
                },
                fixture.Create<LocatieLookupData>() with
                {
                    VCode = VCode2,
                    AdresId = sourceAdresId,
                    LocatieId = 1
                },
                fixture.Create<LocatieLookupData>() with
                {
                    VCode = VCode1,
                    AdresId = sourceAdresId,
                    LocatieId = 2
                }
            ]);
        }
    }
}
