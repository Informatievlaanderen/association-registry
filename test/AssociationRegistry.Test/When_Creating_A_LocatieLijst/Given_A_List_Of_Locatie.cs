namespace AssociationRegistry.Test.When_Creating_A_LocatieLijst;

using Admin.Api.Constants;
using FluentAssertions;
using Locaties;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_List_Of_Locatie
{
    [Fact]
    public void Then_It_Returns_A_Filled_LocatieLijst()
    {
        var listOfLocatie = new List<Locatie>
        {
            Locatie.Create("Kerker", "kerkstraat", "1", "-1", "666", "penoze", "Nederland", true, Locatietypes.Activiteiten),
        };

        var locatieLijst = LocatieLijst.CreateInstance(listOfLocatie);

        locatieLijst.Should().HaveCount(1);
        locatieLijst[0].Should().BeEquivalentTo(listOfLocatie[0]);
    }
}
