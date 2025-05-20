namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Werkingsgebieden;

using AutoFixture;
using FluentAssertions;
using Framework.Helpers;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;

public class Given_A_List_Of_Werkingsgebieden_With_Duplicates
{
    [Fact]
    public void Then_It_Throws_A_DuplicateWerkingsgebiedException()
    {
        var fixture = new Fixture();

        var werkingsgebieden = WellKnownWerkingsgebieden.Provincies
                                                        .OrderBy(_ => fixture.Create<int>())
                                                        .Take(1)
                                                        .Repeat(2)
                                                        .ToArray();

        var ctor = () => Werkingsgebieden.FromArray(werkingsgebieden);

        ctor.Should().Throw<WerkingsgebiedIsDuplicaat>();
    }
}
