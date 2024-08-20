namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.
    RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class To_A_RegistreerVereningUitKboCommand
{
    [Fact]
    public void Then_We_Get_A_RegistreerVerenigingUitKboCommand()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<RegistreerVerenigingUitKboRequest>();

        var actual = request.ToCommand();

        actual.Deconstruct(out var kboNummer);
        ((string)kboNummer).Should().Be(request.KboNummer);
    }
}
