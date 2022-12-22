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
            locatieLijst[0].Should().BeEquivalentTo(listOfLocatie[0]);
        }
    }
}
