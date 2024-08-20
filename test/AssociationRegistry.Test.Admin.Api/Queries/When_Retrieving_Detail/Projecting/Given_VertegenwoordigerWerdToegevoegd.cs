<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Detail/Projector/Given_VertegenwoordigerWerdToegevoegd.cs
namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Detail.Projector;
========
namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Detail.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Detail/Projecting/Given_VertegenwoordigerWerdToegevoegd.cs

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging.Bronnen;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VertegenwoordigerWerdToegevoegd
{
    [Fact]
    public void Then_it_adds_the_vertegenwoordiger_to_the_detail()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var vertegenwoordigerWerdToegevoegd = fixture.Create<TestEvent<VertegenwoordigerWerdToegevoegd>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(vertegenwoordigerWerdToegevoegd, doc);

        var vertegenwoordiger = doc.Vertegenwoordigers.Should()
                                   .ContainSingle(v => v.VertegenwoordigerId == vertegenwoordigerWerdToegevoegd.Data.VertegenwoordigerId)
                                   .Subject;

        vertegenwoordiger.Should().BeEquivalentTo(
            new Vertegenwoordiger
            {
                JsonLdMetadata = new JsonLdMetadata
                {
                    Id = JsonLdType.Vertegenwoordiger.CreateWithIdValues(
                        doc.VCode, vertegenwoordigerWerdToegevoegd.Data.VertegenwoordigerId.ToString()),
                    Type = JsonLdType.Vertegenwoordiger.Type,
                },
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
                VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens
                {
                    JsonLdMetadata = new JsonLdMetadata
                    {
                        Id = JsonLdType.VertegenwoordigerContactgegeven.CreateWithIdValues(
                            doc.VCode, vertegenwoordigerWerdToegevoegd.Data.VertegenwoordigerId.ToString()),
                        Type = JsonLdType.VertegenwoordigerContactgegeven.Type,
                    },
                    IsPrimair = vertegenwoordigerWerdToegevoegd.Data.IsPrimair,
                    Email = vertegenwoordigerWerdToegevoegd.Data.Email,
                    Telefoon = vertegenwoordigerWerdToegevoegd.Data.Telefoon,
                    Mobiel = vertegenwoordigerWerdToegevoegd.Data.Mobiel,
                    SocialMedia = vertegenwoordigerWerdToegevoegd.Data.SocialMedia,
                },
                Bron = Bron.Initiator,
            });

        doc.Vertegenwoordigers.Should().BeInAscendingOrder(v => v.VertegenwoordigerId);
    }
}
