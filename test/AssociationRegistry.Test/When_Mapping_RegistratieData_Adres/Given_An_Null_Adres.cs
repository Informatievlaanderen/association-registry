namespace AssociationRegistry.Test.When_Mapping_RegistratieData_Adres;

using Events;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Null_Adres
{
    [Fact]
    public void Then_It_Returns_A_Null_Adres()
    {
        Registratiedata.Adres.With(null)
                       .Should().BeNull();
    }
}
