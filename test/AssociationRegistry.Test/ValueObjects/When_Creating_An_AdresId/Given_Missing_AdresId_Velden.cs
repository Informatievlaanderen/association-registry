namespace AssociationRegistry.Test.ValueObjects.When_Creating_An_AdresId;

using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Missing_AdresId_Velden
{
    [Theory]
    [InlineData(null, null)]
    [InlineData(null, AdresId.DataVlaanderenAdresPrefix)]
    [InlineData("AR", null)]
    [InlineData("AR", "")]
    [InlineData("AR", "  ")]
    public void Then_It_Throws_An_IncompleteAdresIdException(string adresbroncode, string waarde)
    {
        var ctor = () => AdresId.Create(
            (Adresbron.CanParse(adresbroncode) ? Adresbron.Parse(adresbroncode) : null)!,
            waarde);

        ctor.Should().Throw<AdresIdIsIncompleet>();
    }
}
