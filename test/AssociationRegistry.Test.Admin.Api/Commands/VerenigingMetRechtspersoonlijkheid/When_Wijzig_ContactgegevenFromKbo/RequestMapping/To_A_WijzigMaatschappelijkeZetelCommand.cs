namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_Wijzig_ContactgegevenFromKbo.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.WijzigContactgegeven.RequestModels;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
[Category("Mapping")]
public class To_A_WijzigContactgegevenUitKboCommand
{
    [Fact]
    public void Then_We_Get_A_WijzigContactgegevenUitKboCommand()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<WijzigContactgegevenRequest>();

        var actualVCode = fixture.Create<VCode>();
        var contactgegevenId = fixture.Create<int>();
        var actual = request.ToCommand(actualVCode, contactgegevenId);

        actual.Deconstruct(out var vCode, out var contactgegeven);

        vCode.Should().Be(actualVCode);
        contactgegeven.ContacgegevenId.Should().Be(contactgegevenId);
        contactgegeven.Beschrijving.Should().Be(request.Contactgegeven.Beschrijving);
        contactgegeven.IsPrimair.Should().Be(request.Contactgegeven.IsPrimair);
    }
}
