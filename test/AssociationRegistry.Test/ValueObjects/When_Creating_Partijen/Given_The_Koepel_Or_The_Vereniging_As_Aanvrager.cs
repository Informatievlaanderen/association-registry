namespace AssociationRegistry.Test.ValueObjects.When_Creating_Partijen;

using DecentraalBeheer.Relatieaanvragen;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;

public class Given_The_Koepel_Or_The_Vereniging_As_Aanvrager
{
    private static readonly VCode KoepelVCode = VCode.Create("V0001001");
    private static readonly VCode VerenigingVCode = VCode.Create("V0001002");

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Then_It_Contains_The_Koepel_The_Vereniging_And_The_Aanvrager(bool deKoepelDientIn)
    {
        var aangevraagdDoor = deKoepelDientIn ? KoepelVCode : VerenigingVCode;

        var partijen = Partijen.Create(KoepelVCode, VerenigingVCode, aangevraagdDoor);

        partijen.KoepelVCode.Should().Be(KoepelVCode);
        partijen.VerenigingVCode.Should().Be(VerenigingVCode);
        partijen.AangevraagdDoor.Should().Be(aangevraagdDoor);
    }
}
