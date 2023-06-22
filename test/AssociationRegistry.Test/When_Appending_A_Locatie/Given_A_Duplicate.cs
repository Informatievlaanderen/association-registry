namespace AssociationRegistry.Test.When_Appending_A_Locatie;

using AutoFixture;
using Framework;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;

public class Given_A_Duplicate
{
    [Fact]
    public void Then_it_throws()
    {
        var fixture = new Fixture().CustomizeAll();

        var locatie = fixture.Create<Locatie>();

        var locaties = Locaties.Empty.Append(locatie);

        Assert.Throws<DuplicateLocatieProvided>(() => locaties.Append(locatie));
    }
}
