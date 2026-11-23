namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Dien_Relatieaanvraag_In.RequestMapping;

using AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.DienRelatieaanvraagIn.RequestModels;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.DienRelatieaanvraagIn;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class To_DienRelatieaanvraagInCommand
{
    private static readonly VCode AangevraagdDoor = VCode.Create("V0001001");

    [Fact]
    public void Then_The_Command_Contains_All_Fields_And_The_Aanvrager_From_vr_namens()
    {
        var request = new DienRelatieaanvraagInRequest
        {
            KoepelVCode = "V0001001",
            VerenigingVCode = "V0001002",
            RelatieType = "subvereniging",
            Beschrijving = "Wij zijn de afdeling Bree",
        };

        var command = request.ToCommand(AangevraagdDoor);

        command
            .Should()
            .BeEquivalentTo(
                new DienRelatieaanvraagInCommand(
                    Partijen.Create(VCode.Create("V0001001"), VCode.Create("V0001002"), AangevraagdDoor),
                    RelatieType.Subvereniging,
                    "Wij zijn de afdeling Bree"
                )
            );
    }

    [Fact]
    public void Without_Beschrijving_Then_The_Beschrijving_Is_Empty()
    {
        var command = new DienRelatieaanvraagInRequest
        {
            KoepelVCode = "V0001001",
            VerenigingVCode = "V0001002",
            RelatieType = "Lidmaatschap",
        }.ToCommand(AangevraagdDoor);

        command.Beschrijving.Should().BeEmpty();
    }

    [Theory]
    [InlineData("V1", "V0001002")]
    [InlineData("V0001001", "ABC0001")]
    [InlineData("V0001001", "V0000001")]
    public void With_An_Invalid_VCode_Then_It_Throws_VCodeIsOngeldig(string koepelVCode, string verenigingVCode)
    {
        var request = new DienRelatieaanvraagInRequest
        {
            KoepelVCode = koepelVCode,
            VerenigingVCode = verenigingVCode,
            RelatieType = "Lidmaatschap",
        };

        var toCommand = () => request.ToCommand(AangevraagdDoor);

        toCommand.Should().Throw<VCodeIsOngeldig>();
    }

    [Fact]
    public void With_A_vr_namens_That_Is_No_Partij_Then_It_Throws_AanvragerIsGeenPartijBijRelatieaanvraag()
    {
        // vóór er iets geladen wordt: de request alleen zegt al dat dit niet kan
        var request = new DienRelatieaanvraagInRequest
        {
            KoepelVCode = "V0001001",
            VerenigingVCode = "V0001002",
            RelatieType = "Lidmaatschap",
        };

        var toCommand = () => request.ToCommand(VCode.Create("V0009999"));

        toCommand.Should().Throw<AanvragerIsGeenPartijBijRelatieaanvraag>();
    }

    [Fact]
    public void With_The_Same_Vereniging_As_Koepel_Then_It_Throws_VerenigingKanGeenRelatieMetZichzelfAanvragen()
    {
        var request = new DienRelatieaanvraagInRequest
        {
            KoepelVCode = "V0001001",
            VerenigingVCode = "V0001001",
            RelatieType = "Lidmaatschap",
        };

        var toCommand = () => request.ToCommand(AangevraagdDoor);

        toCommand.Should().Throw<VerenigingKanGeenRelatieMetZichzelfAanvragen>();
    }

    [Theory]
    [InlineData("Onbekend")]
    [InlineData("Lid")]
    [InlineData(" Lidmaatschap")]
    public void With_An_Unknown_RelatieType_Then_It_Throws_RelatieTypeIsOngeldig(string relatieType)
    {
        var request = new DienRelatieaanvraagInRequest
        {
            KoepelVCode = "V0001001",
            VerenigingVCode = "V0001002",
            RelatieType = relatieType,
        };

        var toCommand = () => request.ToCommand(AangevraagdDoor);

        toCommand.Should().Throw<RelatieTypeIsOngeldig>();
    }
}
