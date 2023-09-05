namespace AssociationRegistry.Test.When_Creating_A_Startdatum;

using FluentAssertions;
using Vereniging;
using Vereniging.Einddatum;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Null_Einddatum
{
    [Fact]
    public void Then_it_returns_null()
    {
        var einddatum = Einddatum.Create(null);
        einddatum.Datum.Should().BeNull();
    }
}
