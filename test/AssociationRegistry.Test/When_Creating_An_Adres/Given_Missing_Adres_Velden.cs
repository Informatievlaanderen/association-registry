namespace AssociationRegistry.Test.When_Creating_An_Adres;

using FluentAssertions;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Missing_Adres_Velden
{
    [Theory]
    [InlineData("", "", "", "", "", "")]
    [InlineData("", "huisnummer", "", "postcode", "gemeente", "land")]
    [InlineData("straatnaam", "", "", "postcode", "gemeente", "land")]
    [InlineData("straatnaam", "huisnummer", "", "", "gemeente", "land")]
    [InlineData("straatnaam", "huisnummer", "", "postcode", "", "land")]
    [InlineData("straatnaam", "huisnummer", "", "postcode", "gemeente", "")]
    [InlineData("straatnaam", "", "", "", "gemeente", "")]
    public void Then_It_Throws_An_IncompleteAdresIdException(
        string straatnaam,
        string huisnummer,
        string busnummer,
        string postcode,
        string gemeente,
        string land)
    {
        var ctor = () => Adres.Create(
            straatnaam,
            huisnummer,
            busnummer,
            postcode,
            gemeente,
            land);

        ctor.Should().Throw<AdresIsIncompleet>();
    }
}
