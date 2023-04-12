namespace AssociationRegistry.Test.When_Creating_A_VertegenwoordigersLijst;

using AutoFixture;
using Contactgegevens;
using Framework;
using INSZ;
using Vertegenwoordigers;
using Vertegenwoordigers.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_List_Of_Vertegenwoordigers_With_Same_Insz
{
    [Fact]
    public void Then_It_Throws_A_DuplicateInszProvided()
    {
        var fixture = new Fixture();
        var vertegenwoordiger1 = Vertegenwoordiger.Create(
            Insz.Create(InszTestSet.Insz1),
            false,
            fixture.Create<string>(),
            fixture.Create<string>(),
            fixture.Create<string>(),
            fixture.Create<string>(),
            Contactgegevens.Empty);
        var vertegenwoordiger2 = Vertegenwoordiger.Create(
            Insz.Create(InszTestSet.Insz1),
            false,
            fixture.Create<string>(),
            fixture.Create<string>(),
            fixture.Create<string>(),
            fixture.Create<string>(),
            Contactgegevens.Empty);
        var listOfVertegenwoordigers = new List<Vertegenwoordiger>()
        {
            vertegenwoordiger1,
            vertegenwoordiger2,
        };

        Assert.Throws<DuplicateInszProvided>(() => VertegenwoordigersLijst.Create(listOfVertegenwoordigers));
    }
}
