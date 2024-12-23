namespace AssociationRegistry.Test.Locaties.Adressen.When_Formatting_A_VerrijkteGemeentenaam;

using AssociationRegistry.Events;
using FluentAssertions;
using Xunit;

public class WithoutPostnaam
{
    [Fact]
    public void Then_It_FormatsCorrectly()
    {
        var sut = VerrijkteGemeentenaam.ZonderPostnaam("Affligem");

        sut.Format().Should().BeEquivalentTo("Affligem");
    }
}