namespace AssociationRegistry.Test.Werkingsgebied;

using FluentAssertions;
using Vereniging;
using Xunit;

public class WerkingsgebiedTests
{
    [Fact]
    public void Werkingsgebied_All_Is_Not_Empty()
    {
        Werkingsgebied.AllExamples.Should().NotBeEmpty();
    }
}
