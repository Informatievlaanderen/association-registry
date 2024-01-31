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
public class Given_ContactgegevenUitKBOWerdGewijzigd
{
    [Fact]
    public void Then_it_updates_the_contactgegeven()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<TestEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>>();

        var contactgegevenWerdToegevoegd = fixture.Create<TestEvent<ContactgegevenWerdOvergenomenUitKBO>>();

        var contactgegevenWerdGewijzigd = new TestEvent<ContactgegevenUitKBOWerdGewijzigd>(
            fixture.Create<ContactgegevenUitKBOWerdGewijzigd>() with
            {
                ContactgegevenId = contactgegevenWerdToegevoegd.Data.ContactgegevenId,
            });

        var doc = BeheerVerenigingDetailProjector.Create(verenigingMetRechtspersoonlijkheidWerdGeregistreerd);
        BeheerVerenigingDetailProjector.Apply(contactgegevenWerdToegevoegd, doc);
        BeheerVerenigingDetailProjector.Apply(contactgegevenWerdGewijzigd, doc);

        doc.Contactgegevens.Should().ContainEquivalentOf(
            new Contactgegeven
            {
                JsonLdMetadata = new JsonLdMetadata
                {
                    Id = JsonLdType.Contactgegeven.CreateWithIdValues(doc.VCode, contactgegevenWerdToegevoegd.Data.ContactgegevenId.ToString()),
                    Type = JsonLdType.Contactgegeven.Type,
                },
                ContactgegevenId = contactgegevenWerdToegevoegd.Data.ContactgegevenId,
                Beschrijving = contactgegevenWerdGewijzigd.Data.Beschrijving,
                IsPrimair = contactgegevenWerdGewijzigd.Data.IsPrimair,
                Contactgegeventype = contactgegevenWerdToegevoegd.Data.Contactgegeventype,
                Waarde = contactgegevenWerdToegevoegd.Data.Waarde,
                Bron = Bron.KBO,
            });

        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
    }
}
