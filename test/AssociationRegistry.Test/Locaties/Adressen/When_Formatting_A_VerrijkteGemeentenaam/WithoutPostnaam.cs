namespace AssociationRegistry.Test.Locaties.Adressen.When_Formatting_A_VerrijkteGemeentenaam;

using FluentAssertions;
using GemeentenaamDecorator;
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
