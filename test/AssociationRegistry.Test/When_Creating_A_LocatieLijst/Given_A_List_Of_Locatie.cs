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
            Locatie.Create(naam: Locatienaam.Create("Kerker"), isPrimair: true, Locatietype.Activiteiten, adresId: null,
                           Adres.Create(straatnaam: "kerkstraat", huisnummer: "1", busnummer: "-1", postcode: "666", gemeente: "penoze",
                                        land: "Nederland")),
        };

        var locatieLijst = Locaties.Empty.Hydrate(listOfLocatie);

        locatieLijst.Should().BeEquivalentTo(
            listOfLocatie,
            config: options => options.Excluding(locatie => locatie.LocatieId));
    }
}
