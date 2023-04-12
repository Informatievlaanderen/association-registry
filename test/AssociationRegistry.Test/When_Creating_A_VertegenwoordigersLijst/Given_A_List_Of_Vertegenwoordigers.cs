namespace AssociationRegistry.Test.When_Creating_A_VertegenwoordigersLijst;

using AutoFixture;
using Contactgegevens;
using FluentAssertions;
using Framework;
using INSZ;
using Vertegenwoordigers;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_List_Of_Vertegenwoordigers
{
    [Fact]
    public void Then_It_Returns_A_Filled_VertegenwoordigersLijst()
    {
        var fixture = new Fixture();
        var vertegenwoordiger1 = Vertegenwoordiger.Create(
            Insz.Create(InszTestSet.Insz1),
            true,
            fixture.Create<string>(),
            fixture.Create<string>(),
            fixture.Create<string>(),
            fixture.Create<string>(),
            Contactgegevens.Empty);
        var vertegenwoordiger2 = Vertegenwoordiger.Create(
            Insz.Create(InszTestSet.Insz2),
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

        var vertegenwoordigersLijst = VertegenwoordigersLijst.Create(listOfVertegenwoordigers);

        vertegenwoordigersLijst.Should().HaveCount(2);
        vertegenwoordigersLijst[0].Should().BeEquivalentTo(vertegenwoordiger1);
        vertegenwoordigersLijst[1].Should().BeEquivalentTo(vertegenwoordiger2);
    }
}
