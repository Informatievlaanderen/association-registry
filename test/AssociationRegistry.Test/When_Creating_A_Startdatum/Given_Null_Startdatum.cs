namespace AssociationRegistry.Test.When_Creating_A_Startdatum;

using FluentAssertions;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Null_Startdatum
{
    [Fact]
    public void Then_it_returns_null()
    {
        var startdatum = Startdatum.Create(null);
        startdatum.Datum.Should().BeNull();
    }
}
