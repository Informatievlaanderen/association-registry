namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid;
using Framework;
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
        var fixture = new Fixture().CustomizeAll();

        var request = fixture.Create<RegistreerVerenigingUitKboRequest>();

        var actual = request.ToCommand();

        actual.Deconstruct(out var kboNummer);
        ((string)kboNummer).Should().Be(request.KboNummer);
    }
}
