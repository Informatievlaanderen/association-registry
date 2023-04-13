namespace AssociationRegistry.Test.When_Creating_A_KboNummer;

using FluentAssertions;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Null_KboNummer
{
    [Fact]
    public void Then_it_returns_null()
    {
        var kboNummer = KboNummer.Create(null);

        kboNummer.Value.Should().BeNull();
    }
}
