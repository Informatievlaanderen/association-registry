namespace AssociationRegistry.Test.Locaties.Adressen.When_Formatting_A_VerrijkteGemeentenaam;

using AssociationRegistry.Events;
using AssociationRegistry.Grar.Models.PostalInfo;
using FluentAssertions;
using Xunit;

public class WithPostnaam
{
    [Fact]
    public void Then_It_FormatsCorrectly()
    {
        var sut = VerrijkteGemeentenaam.MetPostnaam(Postnaam.FromValue("Hekelgem"), "Affligem");

        sut.Format().Should().BeEquivalentTo("Hekelgem (Affligem)");
    }
}
