namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
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
        var vertegenwoordigerWerdToegevoegd = fixture.Create<TestEvent<VertegenwoordigerWerdToegevoegd>>();
        var projector = new BeheerVerenigingDetailProjection();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        projector.Apply(vertegenwoordigerWerdToegevoegd, doc);

        doc.Vertegenwoordigers.Should().Contain(
            new BeheerVerenigingDetailDocument.Vertegenwoordiger
            {
                VertegenwoordigerId = vertegenwoordigerWerdToegevoegd.Data.VertegenwoordigerId,
                Insz = vertegenwoordigerWerdToegevoegd.Data.Insz,
                Achternaam = vertegenwoordigerWerdToegevoegd.Data.Achternaam,
                Voornaam = vertegenwoordigerWerdToegevoegd.Data.Voornaam,
                Roepnaam = vertegenwoordigerWerdToegevoegd.Data.Roepnaam,
                Rol = vertegenwoordigerWerdToegevoegd.Data.Rol,
                IsPrimair = vertegenwoordigerWerdToegevoegd.Data.IsPrimair,
                Email = vertegenwoordigerWerdToegevoegd.Data.Email,
                Telefoon = vertegenwoordigerWerdToegevoegd.Data.Telefoon,
                Mobiel = vertegenwoordigerWerdToegevoegd.Data.Mobiel,
                SocialMedia = vertegenwoordigerWerdToegevoegd.Data.SocialMedia,
            });
        doc.DatumLaatsteAanpassing.Should().Be(vertegenwoordigerWerdToegevoegd.Tijdstip.ToBelgianDate());
        doc.Metadata.Should().BeEquivalentTo(new Metadata(vertegenwoordigerWerdToegevoegd.Sequence, vertegenwoordigerWerdToegevoegd.Version));}
}
