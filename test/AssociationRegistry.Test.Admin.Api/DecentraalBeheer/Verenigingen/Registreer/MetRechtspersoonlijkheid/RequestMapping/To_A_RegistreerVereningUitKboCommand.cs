namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.MetRechtspersoonlijkheid.
    RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Test.Common.AutoFixture;
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
