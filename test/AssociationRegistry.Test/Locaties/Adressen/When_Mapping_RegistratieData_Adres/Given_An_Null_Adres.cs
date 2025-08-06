namespace AssociationRegistry.Test.Locaties.Adressen.When_Mapping_RegistratieData_Adres;

using Events.Factories;
using FluentAssertions;
using Xunit;

public class Given_An_Null_Adres
{
    [Fact]
    public void Then_It_Returns_A_Null_Adres()
    {
        EventFactory.Adres(null)
                       .Should().BeNull();
    }
}
