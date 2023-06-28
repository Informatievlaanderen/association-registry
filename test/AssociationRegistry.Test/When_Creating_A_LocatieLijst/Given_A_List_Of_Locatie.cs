namespace AssociationRegistry.Test.When_Creating_A_LocatieLijst;

using FluentAssertions;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_List_Of_Locatie
{
    [Fact]
    public void Then_It_Returns_A_Filled_LocatieLijst()
    {
        var listOfLocatie = new[]
        {
            Locatie.Create("Kerker", true, Locatietype.Activiteiten, null, Adres.Create("kerkstraat", "1", "-1", "666", "penoze", "Nederland")),
        };

        var locatieLijst = Locaties.Empty.Hydrate(listOfLocatie);

        locatieLijst.Should().BeEquivalentTo(
            listOfLocatie,
            options => options.Excluding(locatie => locatie.LocatieId));
    }
}
