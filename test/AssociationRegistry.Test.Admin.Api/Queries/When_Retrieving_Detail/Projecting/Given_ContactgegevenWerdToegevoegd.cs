<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Detail/Projector/Given_ContactgegevenWerdToegevoegd.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Detail.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Detail.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Detail/Projecting/Given_ContactgegevenWerdToegevoegd.cs

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
public class Given_ContactgegevenWerdToegevoegd
{
    [Fact]
    public void Then_it_adds_the_contactgegeven_to_the_detail()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var contactgegevenWerdToegevoegd = fixture.Create<TestEvent<ContactgegevenWerdToegevoegd>>();
        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(contactgegevenWerdToegevoegd, doc);

        doc.Contactgegevens.Should().ContainEquivalentOf(
            new Contactgegeven
            {
                JsonLdMetadata = new JsonLdMetadata
                {
                    Id = JsonLdType.Contactgegeven.CreateWithIdValues(
                        doc.VCode, contactgegevenWerdToegevoegd.Data.ContactgegevenId.ToString()),
                    Type = JsonLdType.Contactgegeven.Type,
                },
                ContactgegevenId = contactgegevenWerdToegevoegd.Data.ContactgegevenId,
                Contactgegeventype = contactgegevenWerdToegevoegd.Data.Contactgegeventype,
                Waarde = contactgegevenWerdToegevoegd.Data.Waarde,
                Beschrijving = contactgegevenWerdToegevoegd.Data.Beschrijving,
                IsPrimair = contactgegevenWerdToegevoegd.Data.IsPrimair,
                Bron = Bron.Initiator,
            });

        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
    }
}
