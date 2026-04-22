namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.RequestMapping;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning.RequestModels;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.WijzigVertegenwoordiger.RequestModels;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_A_Valid_Request
{
    [Fact]
    public void Then_We_Get_A_Correct_Command()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<RegistreerErkenningRequest>();

        var vCode = fixture.Create<VCode>();
        var command = request.ToCommand(vCode);

        var expectedErkenningsPeriode = ErkenningsPeriode.Create(
            request.Erkenning.Startdatum,
            request.Erkenning.Einddatum
        );
        command.VCode.Should().Be(vCode);
        command.Erkenning.IpdcProductNummer.Should().Be(request.Erkenning.IpdcProductNummer);
        command.Erkenning.ErkenningsPeriode.Should().Be(expectedErkenningsPeriode);
        command.Erkenning.HernieuwingsUrl.Should().Be(HernieuwingsUrl.Create(request.Erkenning.HernieuwingsUrl));

        command
            .Erkenning.Hernieuwingsdatum.Should()
            .BeEquivalentTo(Hernieuwingsdatum.Create(request.Erkenning.Hernieuwingsdatum, expectedErkenningsPeriode));
    }
}
