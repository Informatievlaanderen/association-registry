namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Schorsing_Erkenning.RequestMapping;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerSchorsingErkenning.RequestModels;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Erkenningen.SchorsErkenning.RequestModels;
using AssociationRegistry.DecentraalBeheer.Vereniging;
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

        var request = fixture.Create<CorrigeerSchorsingErkenningRequest>();

        var vCode = fixture.Create<VCode>();
        var erkenningId = fixture.Create<int>();
        var command = request.ToCommand(vCode, erkenningId);

        command.VCode.Should().Be(vCode);
        command.Erkenning.RedenSchorsing.Should().Be(request.RedenSchorsing);
    }
}
