namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.RequestMapping;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.RequestModels;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Xunit;

public class To_VoegBankrekeningnummerToeCommand
{
    [Fact]
    public void Then_We_Get_A_Correct_Command()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<VoegBankrekeningnummerToeRequest>();

        var vCode = fixture.Create<VCode>();
        var command = request.ToCommand(vCode);

        command.VCode.Should().Be(vCode);
        command.Bankrekeningnummer.Iban.Should().Be(IbanNummer.Create(request.Bankrekeningnummer.Iban));
        command.Bankrekeningnummer.Doel.Should().Be(request.Bankrekeningnummer.Doel);
        command.Bankrekeningnummer.Titularis.Value.Should().Be(request.Bankrekeningnummer.Titularis);
    }

    [Fact]
    public void With_Null_Values_For_Doel_Then_Set_String_Empty()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<VoegBankrekeningnummerToeRequest>();
        request.Bankrekeningnummer.Doel = null;

        var vCode = fixture.Create<VCode>();
        var command = request.ToCommand(vCode);

        command.VCode.Should().Be(vCode);
        command.Bankrekeningnummer.Doel.Should().Be(string.Empty);
    }
}
