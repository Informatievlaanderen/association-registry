namespace AssociationRegistry.Test.ValueObjects.When_Creating_Partijen;

using DecentraalBeheer.Relatieaanvragen;
using DecentraalBeheer.Relatieaanvragen.Exceptions;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;

public class Given_An_Aanvrager_That_Is_No_Partij
{
    [Fact]
    public void Then_It_Throws_AanvragerIsGeenPartijBijRelatieaanvraag()
    {
        var create = () =>
            Partijen.Create(
                VCode.Create("V0001001"),
                VCode.Create("V0001002"),
                aangevraagdDoor: VCode.Create("V0009999")
            );

        create.Should().Throw<AanvragerIsGeenPartijBijRelatieaanvraag>();
    }
}
