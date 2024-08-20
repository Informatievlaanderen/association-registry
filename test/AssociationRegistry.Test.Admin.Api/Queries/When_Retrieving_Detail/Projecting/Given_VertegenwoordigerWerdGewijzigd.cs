<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Detail/Projector/Given_VertegenwoordigerWerdGewijzigd.cs
namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Detail.Projector;
========
namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Detail.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Detail/Projecting/Given_VertegenwoordigerWerdGewijzigd.cs

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VertegenwoordigerWerdGewijzigd
{
    [Fact]
    public void Then_it_updates_the_vertegenwoordiger_in_the_detail()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var vert = fixture.Create<Vertegenwoordiger>();

        var vertegenwoordigerWerdGewijzigd = new TestEvent<VertegenwoordigerWerdGewijzigd>(
            fixture.Create<VertegenwoordigerWerdGewijzigd>() with
            {
                VertegenwoordigerId = vert.VertegenwoordigerId,
            });

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        doc.Vertegenwoordigers = doc.Vertegenwoordigers.Append(
            vert
        ).ToArray();

        BeheerVerenigingDetailProjector.Apply(vertegenwoordigerWerdGewijzigd, doc);

        var vertegenwoordiger = doc.Vertegenwoordigers.Should().ContainSingle(v => v.VertegenwoordigerId == vert.VertegenwoordigerId)
                                   .Subject;

        vertegenwoordiger.Should().BeEquivalentTo(
            new Vertegenwoordiger
            {
                JsonLdMetadata = vert.JsonLdMetadata,
                VertegenwoordigerId = vertegenwoordigerWerdGewijzigd.Data.VertegenwoordigerId,
                Achternaam = vert.Achternaam,
                Voornaam = vert.Voornaam,
                Insz = vert.Insz,
                Roepnaam = vertegenwoordigerWerdGewijzigd.Data.Roepnaam,
                Rol = vertegenwoordigerWerdGewijzigd.Data.Rol,
                IsPrimair = vertegenwoordigerWerdGewijzigd.Data.IsPrimair,
                Email = vertegenwoordigerWerdGewijzigd.Data.Email,
                Telefoon = vertegenwoordigerWerdGewijzigd.Data.Telefoon,
                Mobiel = vertegenwoordigerWerdGewijzigd.Data.Mobiel,
                SocialMedia = vertegenwoordigerWerdGewijzigd.Data.SocialMedia,
                VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens
                {
                    JsonLdMetadata = vert.VertegenwoordigerContactgegevens.JsonLdMetadata,
                    IsPrimair = vertegenwoordigerWerdGewijzigd.Data.IsPrimair,
                    Email = vertegenwoordigerWerdGewijzigd.Data.Email,
                    Telefoon = vertegenwoordigerWerdGewijzigd.Data.Telefoon,
                    Mobiel = vertegenwoordigerWerdGewijzigd.Data.Mobiel,
                    SocialMedia = vertegenwoordigerWerdGewijzigd.Data.SocialMedia,
                },
                Bron = vert.Bron,
            });

        doc.Vertegenwoordigers.Should().BeInAscendingOrder(v => v.VertegenwoordigerId);
    }
}
