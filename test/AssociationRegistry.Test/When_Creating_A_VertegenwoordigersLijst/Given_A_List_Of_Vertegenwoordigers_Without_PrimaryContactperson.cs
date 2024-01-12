﻿namespace AssociationRegistry.Test.When_Creating_A_VertegenwoordigersLijst;

using AutoFixture;
using FluentAssertions;
using Framework.Customizations;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_List_Of_Vertegenwoordigers_Without_PrimaryContactperson
{
    [Fact]
    public void Then_It_Returns_A_Filled_VertegenwoordigersLijst()
    {
        var fixture = new Fixture().CustomizeDomain();
        var vertegenwoordiger1 = fixture.Create<Vertegenwoordiger>() with { IsPrimair = false };
        var vertegenwoordiger2 = fixture.Create<Vertegenwoordiger>() with { IsPrimair = false };

        var listOfVertegenwoordigers = new[]
        {
            vertegenwoordiger1,
            vertegenwoordiger2,
        };

        var vertegenwoordigersLijst = Vertegenwoordigers.Empty.VoegToe(listOfVertegenwoordigers);

        vertegenwoordigersLijst.Should().HaveCount(expected: 2);

        vertegenwoordigersLijst.Should().BeEquivalentTo(
            listOfVertegenwoordigers,
            config: options => options.Excluding(x => x.VertegenwoordigerId));
    }
}
