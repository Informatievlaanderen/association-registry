namespace AssociationRegistry.Test.When_Formatting_A_VerrijkteGemeentenaam;

using Events;
using FluentAssertions;
using Grar.Models.PostalInfo;
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
