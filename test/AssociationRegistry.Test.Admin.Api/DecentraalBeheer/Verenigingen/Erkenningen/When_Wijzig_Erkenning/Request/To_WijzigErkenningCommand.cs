namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.Request;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.WijzigErkenning.RequestModels;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Xunit;

public class To_WijzigErkenningCommand
{
    [Fact]
    public void Then_Map_To_Command()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<WijzigErkenningRequest>();

        var vCode = fixture.Create<VCode>();
        var erkenningId = fixture.Create<int>();
        var command = request.ToCommand(vCode, erkenningId);

        command.VCode.Should().Be(vCode);
        command.Erkenning.ErkenningId.Should().Be(erkenningId);
        command.Erkenning.StartDatum.Should().Be(request.Startdatum);
        command.Erkenning.EindDatum.Should().Be(request.Einddatum);
        command.Erkenning.Hernieuwingsdatum.Should().Be(request.Hernieuwingsdatum);
        command.Erkenning.HernieuwingsUrl.Should().Be(request.HernieuwingsUrl);
        command.Erkenning.RedenVanWijziging.Should().Be(request.RedenVanWijziging);
    }
}
