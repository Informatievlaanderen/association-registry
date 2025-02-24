namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_Detail.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging.Bronnen;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenWerdOvergenomenUitKBO
{
    [Fact]
    public void Then_it_adds_the_contactgegeven_to_the_detail()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var contactgegevenWerdToegevoegd = fixture.Create<TestEvent<ContactgegevenWerdOvergenomenUitKBO>>();
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
                Beschrijving = string.Empty,
                Contactgegeventype = contactgegevenWerdToegevoegd.Data.Contactgegeventype,
                Waarde = contactgegevenWerdToegevoegd.Data.Waarde,
                Bron = Bron.KBO,
            });

        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
    }
}

[UnitTest]
public class Given_ContactgegevenWerdGewijzigdUitKbo
{
    [Fact]
    public void Then_it_updates_the_contactgegeven_in_the_detail()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var contactgegevenWerdGewijzigdUitKbo = fixture.Create<TestEvent<ContactgegevenWerdGewijzigdInKbo>>();
        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        var contactgegevenBeforeEdit = new Contactgegeven
        {
            JsonLdMetadata = new JsonLdMetadata
            {
                Id = JsonLdType.Contactgegeven.CreateWithIdValues(
                    doc.VCode, contactgegevenWerdGewijzigdUitKbo.Data.ContactgegevenId.ToString()),
                Type = JsonLdType.Contactgegeven.Type,
            },
            ContactgegevenId = contactgegevenWerdGewijzigdUitKbo.Data.ContactgegevenId,
            Contactgegeventype = contactgegevenWerdGewijzigdUitKbo.Data.Contactgegeventype,
            Waarde = fixture.Create<string>(),
            Beschrijving = fixture.Create<string>(),
            IsPrimair = true,
            Bron = Bron.KBO,
        };

        doc.Contactgegevens = new[]
        {
            contactgegevenBeforeEdit,
        };

        BeheerVerenigingDetailProjector.Apply(contactgegevenWerdGewijzigdUitKbo, doc);

        doc.Contactgegevens.Should().ContainEquivalentOf(
            new Contactgegeven
            {
                JsonLdMetadata = contactgegevenBeforeEdit.JsonLdMetadata,
                ContactgegevenId = contactgegevenBeforeEdit.ContactgegevenId,
                Beschrijving = contactgegevenBeforeEdit.Beschrijving,
                Contactgegeventype = contactgegevenBeforeEdit.Contactgegeventype,
                Waarde = contactgegevenWerdGewijzigdUitKbo.Data.Waarde,
                IsPrimair = contactgegevenBeforeEdit.IsPrimair,
                Bron = Bron.KBO,
            });

        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
    }
}
