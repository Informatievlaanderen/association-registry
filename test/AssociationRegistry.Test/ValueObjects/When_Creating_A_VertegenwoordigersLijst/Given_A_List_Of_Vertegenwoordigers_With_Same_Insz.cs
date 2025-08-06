namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_VertegenwoordigersLijst;

using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using Xunit;

public class Given_A_List_Of_Vertegenwoordigers_With_Same_Insz
{
    [Fact]
    public void Then_It_Throws_A_DuplicateInszProvided()
    {
        var fixture = new Fixture().CustomizeDomain();
        var vertegenwoordiger1 = fixture.Create<Vertegenwoordiger>();
        var vertegenwoordiger2 = fixture.Create<Vertegenwoordiger>() with { Insz = vertegenwoordiger1.Insz };

        var listOfVertegenwoordigers = new[]
        {
            vertegenwoordiger1,
            vertegenwoordiger2,
        };

        Assert.Throws<InszMoetUniekZijn>(() => Vertegenwoordigers.Empty.VoegToe(listOfVertegenwoordigers));
    }
}
