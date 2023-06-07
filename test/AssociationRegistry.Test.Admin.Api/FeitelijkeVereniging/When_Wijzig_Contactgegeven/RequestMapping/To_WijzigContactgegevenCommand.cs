﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Wijzig_Contactgegeven.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven;
using Framework;
using Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
[Category("Mapping")]
public class To_WijzigContactgegevenCommand
{
    [Fact]
    public void Then_We_Get_A_Correct_Command()
    {
        var fixture = new Fixture().CustomizeAll();

        var request = fixture.Create<WijzigContactgegevenRequest>();

        var vCode = fixture.Create<VCode>();
        var contactgegevenId = fixture.Create<int>();
        var command = request.ToCommand(vCode, contactgegevenId);

        command.VCode.Should().Be(vCode);
        command.Contactgegeven.ContacgegevenId.Should().Be(contactgegevenId);
        command.Contactgegeven.Waarde.Should().Be(request.Contactgegeven.Waarde);
        command.Contactgegeven.Beschrijving.Should().Be(request.Contactgegeven.Beschrijving);
        command.Contactgegeven.IsPrimair.Should().Be(request.Contactgegeven.IsPrimair);
    }
}
