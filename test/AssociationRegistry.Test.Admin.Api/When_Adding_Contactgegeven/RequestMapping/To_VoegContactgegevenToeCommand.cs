namespace AssociationRegistry.Test.Admin.Api.When_Adding_Contactgegeven.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VoegContactGegevenToe;
using Framework;
using VCodes;
using AutoFixture;
using Contactgegevens;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
[Category("Mapping")]
public class To_VoegContactgegevenToeCommand
{
    [Fact]
    public void Then_We_Get_A_Correct_Command()
    {
        var fixture = new Fixture().CustomizeAll();

        var request = fixture.Create<VoegContactgegevenToeRequest>();

        var vCode = fixture.Create<VCode>();
        var command = request.ToCommand(vCode);


        command.VCode.Should().Be(vCode);
        command.Contactgegeven.Type.Should().Be(ContactgegevenType.Parse(request.Contactgegeven.Type));
        command.Contactgegeven.Waarde.Should().Be(request.Contactgegeven.Waarde);
        command.Contactgegeven.Omschrijving.Should().Be(request.Contactgegeven.Omschrijving);
        command.Contactgegeven.IsPrimair.Should().Be(request.Contactgegeven.IsPrimair);
    }
}
