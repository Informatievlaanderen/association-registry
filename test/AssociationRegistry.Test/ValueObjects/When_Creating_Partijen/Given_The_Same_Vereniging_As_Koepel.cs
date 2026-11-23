namespace AssociationRegistry.Test.ValueObjects.When_Creating_Partijen;

using DecentraalBeheer.Relatieaanvragen;
using DecentraalBeheer.Relatieaanvragen.Exceptions;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;

public class Given_The_Same_Vereniging_As_Koepel
{
    [Fact]
    public void Then_It_Throws_VerenigingKanGeenRelatieMetZichzelfAanvragen()
    {
        var vCode = VCode.Create("V0001001");

        var create = () => Partijen.Create(vCode, vCode, aangevraagdDoor: vCode);

        create.Should().Throw<VerenigingKanGeenRelatieMetZichzelfAanvragen>();
    }
}
