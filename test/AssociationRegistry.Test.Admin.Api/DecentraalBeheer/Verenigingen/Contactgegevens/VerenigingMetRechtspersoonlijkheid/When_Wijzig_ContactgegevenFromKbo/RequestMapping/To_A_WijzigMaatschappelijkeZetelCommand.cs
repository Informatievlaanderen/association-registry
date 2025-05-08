namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.When_Wijzig_ContactgegevenFromKbo.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.WijzigContactgegeven.RequestModels;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;

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
