﻿namespace AssociationRegistry.Test.When_Creating_A_VertegenwoordigersLijst;

using AutoFixture;
using Framework;
using INSZ;
using Vertegenwoordigers;
using Vertegenwoordigers.Exceptions;
using Xunit;

public class Given_A_List_Of_Vertegenwoordiger_With_Multiple_PrimaryContact_True
{
    [Fact]
    public void Then_It_Throws_A_MultiplePrimaryContacts()
    {
        var fixture = new Fixture();
        var vertegenwoordiger1 = Vertegenwoordiger.Create(Insz.Create(InszTestSet.Insz1), true, fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>());
        var vertegenwoordiger2 = Vertegenwoordiger.Create(Insz.Create(InszTestSet.Insz2), true, fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>());
        var listOfVertegenwoordigers = new List<Vertegenwoordiger>()
        {
            vertegenwoordiger1,
            vertegenwoordiger2,
        };

        Assert.Throws<MultiplePrimaryContacts>(() => VertegenwoordigersLijst.Create(listOfVertegenwoordigers));
    }
}
