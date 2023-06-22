namespace AssociationRegistry.Test.When_Creating_An_AdresId;

using FluentAssertions;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Non_DataVlaanderen_Uri_For_AR_Bron
{
    [Fact]
    public void Then_It_Throws_InvalidBronwaardeForAR()
    {
        var ctor = () => AdresId.Create(
            adresbron: Adresbron.AR,
            bronwaarde: "waarde");
        ctor.Should().Throw<InvalidBronwaardeForAR>();
    }
}
