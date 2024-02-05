﻿namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using JsonLdContext;
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

        var vertegenwoordiger =doc.Vertegenwoordigers.Should().ContainSingle(v=>v.VertegenwoordigerId == vertegenwoordigerWerdOvergenomenUitKbo.Data.VertegenwoordigerId)
                                  .Subject;
        vertegenwoordiger.Should().BeEquivalentTo(
            new Vertegenwoordiger
            {
                JsonLdMetadata = new JsonLdMetadata
                {
                    Id = JsonLdType.Vertegenwoordiger.CreateWithIdValues(doc.VCode, vertegenwoordigerWerdOvergenomenUitKbo.Data.VertegenwoordigerId.ToString()),
                    Type = JsonLdType.Vertegenwoordiger.Type,
                },
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
                VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens
                {
                    JsonLdMetadata = new JsonLdMetadata
                    {
                        Id = JsonLdType.VertegenwoordigerContactgegeven.CreateWithIdValues(
                            doc.VCode, vertegenwoordigerWerdOvergenomenUitKbo.Data.VertegenwoordigerId.ToString()),
                        Type = JsonLdType.VertegenwoordigerContactgegeven.Type,
                    },
                    IsPrimair = false,
                    Email = "",
                    Telefoon = "",
                    Mobiel = "",
                    SocialMedia = "",
                },
                Bron = Bron.KBO,
            });

        doc.Vertegenwoordigers.Should().BeInAscendingOrder(v => v.VertegenwoordigerId);
    }
}
