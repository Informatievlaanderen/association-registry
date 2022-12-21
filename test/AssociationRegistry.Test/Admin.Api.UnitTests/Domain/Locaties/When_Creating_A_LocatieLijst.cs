namespace AssociationRegistry.Test.Admin.Api.UnitTests.Domain.Locaties;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Locaties;
using FluentAssertions;
using Xunit;

public class When_Creating_A_LocatieLijst
{
    public class Given_Null
    {
        [Fact]
        public void Then_It_Returns_Empty()
        {
            var locaties = LocatieLijst.CreateInstance(null);
            locaties.Should().BeEmpty();
        }
    }

    public class Given_An_Empty_Array
    {
        [Fact]
        public void Then_It_Returns_Empty()
        {
            var locaties = LocatieLijst.CreateInstance(Array.Empty<Locatie>());
            locaties.Should().BeEmpty();
        }
    }

    public class Given_A_List_Of_Locatie
    {
        [Fact]
        public void Then_It_Returns_A_Filled_LocatieLijst()
        {
            var listOfLocatie = new List<Locatie>
            {
                Locatie.CreateInstance("Kerker", "kerkstraat", "1", "-1", "666", "penoze", "Nederland", true, LocatieTypes.Activiteiten),
            };

            var locatieLijst = LocatieLijst.CreateInstance(listOfLocatie);

            locatieLijst.Should().HaveCount(1);
            locatieLijst[0].IsHoofdlocatie.Should().BeTrue();
            locatieLijst[0].Huisnummer.Should().Be("1");
            locatieLijst[0].LocatieType.Should().Be(LocatieTypes.Activiteiten);
            locatieLijst[0].Naam.Should().Be("Kerker");
            locatieLijst[0].Busnummer.Should().Be("-1");
            locatieLijst[0].Gemeente.Should().Be("penoze");
            locatieLijst[0].Land.Should().Be("Nederland");
            locatieLijst[0].Postcode.Should().Be("666");
            locatieLijst[0].Straatnaam.Should().Be("kerkstraat");
        }
    }
}
