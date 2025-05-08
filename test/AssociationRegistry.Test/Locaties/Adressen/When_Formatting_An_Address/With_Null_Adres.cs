namespace AssociationRegistry.Test.Locaties.Adressen.When_Formatting_An_Address;

using AssociationRegistry.Events;
using AssociationRegistry.Formats;
using FluentAssertions;
using Xunit;

public class With_Null_Adres
{
    [Fact]
    public void Then_It_Returns_Empty_String()
    {
        Registratiedata.Adres? adres = null;
        adres.ToAdresString().Should().BeEmpty();
    }
}
