namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Vereniging.Bronnen;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VertegenwoordigerWerdOvergenomenUitKBO
{
    [Fact]
    public void Then_it_adds_the_vertegenwoordiger_to_the_detail()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var vertegenwoordigerWerdOvergenomenUitKbo = fixture.Create<TestEvent<VertegenwoordigerWerdOvergenomenUitKBO>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(vertegenwoordigerWerdOvergenomenUitKbo, doc);

        doc.Vertegenwoordigers.Should().Contain(
            new BeheerVerenigingDetailDocument.Vertegenwoordiger
            {
                VertegenwoordigerId = vertegenwoordigerWerdOvergenomenUitKbo.Data.VertegenwoordigerId,
                Insz = vertegenwoordigerWerdOvergenomenUitKbo.Data.Insz,
                Achternaam = vertegenwoordigerWerdOvergenomenUitKbo.Data.Achternaam,
                Voornaam = vertegenwoordigerWerdOvergenomenUitKbo.Data.Voornaam,
                Roepnaam = "",
                Rol = "",
                IsPrimair = false,
                Email = "",
                Telefoon = "",
                Mobiel = "",
                SocialMedia = "",
                Bron = Bron.KBO,
            });

        doc.Vertegenwoordigers.Should().BeInAscendingOrder(v => v.VertegenwoordigerId);
    }
}
