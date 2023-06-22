namespace AssociationRegistry.Test.When_Appending_A_Locatie;

using AutoFixture;
using Framework;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Second_Primaire_Locatie
{
    [Fact]
    public void Then_it_throws()
    {
        var fixture = new Fixture().CustomizeAll();

        var primaireLocatie = fixture.Create<Locatie>() with { IsPrimair = true };
        var locaties = Locaties.Empty.Append(primaireLocatie);

        var locatie = fixture.Create<Locatie>() with { IsPrimair = true };

        Assert.Throws<DuplicatePrimaireLocatieProvided>(() => locaties.Append(locatie));
    }
}
