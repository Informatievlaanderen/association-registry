﻿namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Werkingsgebieden;

using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_A_List_Of_Werkingsgebieden
{
    [Fact]
    public void Then_It_Returns_A_Filled_Werkingsgebieden()
    {
        var fixture = new Fixture();

        var werkingsgebieden = WellKnownWerkingsgebieden.Provincies
                                                        .OrderBy(_ => fixture.Create<int>())
                                                        .Take(2)
                                                        .ToArray();

        Werkingsgebieden.FromArray(werkingsgebieden).Should().BeEquivalentTo(werkingsgebieden);
    }
}
