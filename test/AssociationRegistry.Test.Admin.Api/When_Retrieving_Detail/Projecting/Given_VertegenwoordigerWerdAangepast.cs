namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Projections.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using JasperFx.Core;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VertegenwoordigerWerdGewijzigd
{
    [Fact]
    public void Then_it_updates_the_vertegenwoordiger_in_the_detail()
    {
        var fixture = new Fixture().CustomizeAll();
        var vertegenwoordigerWerdGewijzigd = fixture.Create<TestEvent<VertegenwoordigerWerdGewijzigd>>();
        var projector = new BeheerVerenigingDetailProjection();

        var vertegenwoordiger = fixture.Create<BeheerVerenigingDetailDocument.Vertegenwoordiger>() with
        {
            VertegenwoordigerId = vertegenwoordigerWerdGewijzigd.Data.VertegenwoordigerId,
        };

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();
        doc.Vertegenwoordigers = doc.Vertegenwoordigers.Append(
            vertegenwoordiger
        ).ToArray();

        projector.Apply(vertegenwoordigerWerdGewijzigd, doc);

        doc.Vertegenwoordigers.Should().Contain(
            new BeheerVerenigingDetailDocument.Vertegenwoordiger
            {
                VertegenwoordigerId = vertegenwoordigerWerdGewijzigd.Data.VertegenwoordigerId,
                Achternaam = vertegenwoordiger.Achternaam,
                Voornaam = vertegenwoordiger.Voornaam,
                Roepnaam = vertegenwoordigerWerdGewijzigd.Data.Roepnaam,
                Rol = vertegenwoordigerWerdGewijzigd.Data.Rol,
                IsPrimair = vertegenwoordigerWerdGewijzigd.Data.IsPrimair,
                Email = vertegenwoordigerWerdGewijzigd.Data.Email,
                Telefoon = vertegenwoordigerWerdGewijzigd.Data.Telefoon,
                Mobiel = vertegenwoordigerWerdGewijzigd.Data.Mobiel,
                SocialMedia = vertegenwoordigerWerdGewijzigd.Data.SocialMedia,
            });
        doc.DatumLaatsteAanpassing.Should().Be(vertegenwoordigerWerdGewijzigd.Tijdstip.ToBelgianDate());
        doc.Metadata.Should().BeEquivalentTo(new Metadata(vertegenwoordigerWerdGewijzigd.Sequence, vertegenwoordigerWerdGewijzigd.Version));}
}
