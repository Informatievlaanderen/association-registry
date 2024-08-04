namespace AssociationRegistry.Test.When_Formatting_An_Address;

using Events;
using FluentAssertions;
using Formats;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Null_Adres
{
    [Fact]
    public void Then_It_Returns_Empty_String()
    {
        Registratiedata.Adres? adres = null;
        adres.ToAdresString().Should().BeEmpty();
    }
}
