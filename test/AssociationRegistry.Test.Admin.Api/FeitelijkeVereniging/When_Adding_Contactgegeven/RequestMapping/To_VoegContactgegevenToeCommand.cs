﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Adding_Contactgegeven.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe.RequestsModels;
using Framework;
using Vereniging;
using AutoFixture;
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
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<VoegContactgegevenToeRequest>();

        var vCode = fixture.Create<VCode>();
        var command = request.ToCommand(vCode);


        command.VCode.Should().Be(vCode);
        command.Contactgegeven.Type.Should().Be(ContactgegevenType.Parse(request.Contactgegeven.Type));
        command.Contactgegeven.Waarde.Should().Be(request.Contactgegeven.Waarde);
        command.Contactgegeven.Beschrijving.Should().Be(request.Contactgegeven.Beschrijving);
        command.Contactgegeven.IsPrimair.Should().Be(request.Contactgegeven.IsPrimair);
    }
}
