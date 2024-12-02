namespace AssociationRegistry.Test.Admin.Api.GrarConsumer.FusieEvents.When_Grouping_LocatieLookupData;

using AssociationRegistry.Admin.Api.GrarConsumer.Finders;
using AssociationRegistry.Admin.Api.GrarConsumer.Handlers.StraatHernummering.Groupers;
using AssociationRegistry.Grar.Models;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Grar.GrarUpdates.Fusies.TeHeradresserenLocaties;
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

    public class LocatieLookupTestData : List<LocatieMetVCode>
    {
        public string VCode1 => "1";
        public string VCode2 => "2";

        public LocatieLookupTestData For(string vCode)
        {
            return new(this.Where(x => x.VCode == vCode)
                           .ToArray());
        }

        public TeHeradresserenLocatie[] MapLocatieData(Func<LocatieMetVCode, TeHeradresserenLocatie> map)
            => this.Select(map).ToArray();

        private LocatieLookupTestData(IEnumerable<LocatieMetVCode> data) : base(data)
        {
        }

        public LocatieLookupTestData(IFixture fixture, string sourceAdresId)
        {
            AddRange(
            [
                fixture.Create<LocatieMetVCode>() with
                {
                    VCode = VCode1,
                    LocatieId = 1
                },
                fixture.Create<LocatieMetVCode>() with
                {
                    VCode = VCode2,
                    LocatieId = 1
                },
                fixture.Create<LocatieMetVCode>() with
                {
                    VCode = VCode1,
                    LocatieId = 2
                }
            ]);
        }
    }
}
