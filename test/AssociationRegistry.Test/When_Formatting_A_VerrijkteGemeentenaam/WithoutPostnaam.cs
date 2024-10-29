namespace AssociationRegistry.Test.When_Formatting_A_VerrijkteGemeentenaam;

using Events;
using FluentAssertions;
using Grar.Models.PostalInfo;
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
