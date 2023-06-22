namespace AssociationRegistry.Test.When_Creating_An_AdresId;

using FluentAssertions;
using Vereniging;
using Vereniging.Exceptions;
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
            adresbron: (Adresbron.CanParse(adresbroncode) ? Adresbron.Parse(adresbroncode) : null)!,
            bronwaarde: waarde);
        ctor.Should().Throw<IncompleteAdresId>();
    }
}
