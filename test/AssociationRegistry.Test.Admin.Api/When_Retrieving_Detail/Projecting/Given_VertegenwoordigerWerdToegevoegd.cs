namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Projections.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VertegenwoordigerWerdToegevoegd
{
    [Fact]
    public void Then_it_adds_the_vertegenwoordiger_to_the_detail()
    {
        var fixture = new Fixture().CustomizeAll();
        var vertegenwoordigerWerdToegevoegd = fixture.Create<VertegenwoordigerWerdToegevoegd>();

        var detailDocument = When<VertegenwoordigerWerdToegevoegd>
            .Applying(_ => vertegenwoordigerWerdToegevoegd)
            .ToDetailProjectie();

        detailDocument.Vertegenwoordigers.Should().Contain(
            new BeheerVerenigingDetailDocument.Vertegenwoordiger
            {
                VertegenwoordigerId = vertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                Insz = vertegenwoordigerWerdToegevoegd.Insz,
                Achternaam = vertegenwoordigerWerdToegevoegd.Achternaam,
                Voornaam = vertegenwoordigerWerdToegevoegd.Voornaam,
                Roepnaam = vertegenwoordigerWerdToegevoegd.Roepnaam,
                Rol = vertegenwoordigerWerdToegevoegd.Rol,
                IsPrimair = vertegenwoordigerWerdToegevoegd.IsPrimair,
                Email = vertegenwoordigerWerdToegevoegd.Email,
                Telefoon = vertegenwoordigerWerdToegevoegd.Telefoon,
                Mobiel = vertegenwoordigerWerdToegevoegd.Mobiel,
                SocialMedia = vertegenwoordigerWerdToegevoegd.SocialMedia,
            });
    }
}
