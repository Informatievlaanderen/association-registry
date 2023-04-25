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
                IsPrimair = vertegenwoordigerWerdToegevoegd.IsPrimair,
                Achternaam = vertegenwoordigerWerdToegevoegd.Achternaam,
                Voornaam = vertegenwoordigerWerdToegevoegd.Voornaam,
                Email = vertegenwoordigerWerdToegevoegd.Email,
                Insz = vertegenwoordigerWerdToegevoegd.Insz,
                Mobiel = vertegenwoordigerWerdToegevoegd.Mobiel,
                Roepnaam = vertegenwoordigerWerdToegevoegd.Roepnaam,
                Rol = vertegenwoordigerWerdToegevoegd.Rol,
                SocialMedia = vertegenwoordigerWerdToegevoegd.SocialMedia,
            });
    }
}
