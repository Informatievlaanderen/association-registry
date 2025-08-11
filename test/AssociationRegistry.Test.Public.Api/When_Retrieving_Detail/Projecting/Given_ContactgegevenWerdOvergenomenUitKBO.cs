namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Contracts.JsonLdContext;
using Events;
using FluentAssertions;
using Framework;
using Xunit;

public class Given_ContactgegevenWerdOvergenomenUitKBO
{
    [Fact]
    public void Then_it_adds_the_contactgegeven_to_the_detail()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var contactgegevenWerdOvergenomenUitKbo = fixture.Create<TestEvent<ContactgegevenWerdOvergenomenUitKBO>>();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        PubliekVerenigingDetailProjector.Apply(contactgegevenWerdOvergenomenUitKbo, doc);

        doc.Contactgegevens.Should()
           .ContainSingle(c => c.ContactgegevenId == contactgegevenWerdOvergenomenUitKbo.Data.ContactgegevenId)
           .Which.Should().BeEquivalentTo(
                new PubliekVerenigingDetailDocument.Types.Contactgegeven
                {
                    JsonLdMetadata = new JsonLdMetadata(
                        JsonLdType.Contactgegeven.CreateWithIdValues(contactgegevenWerdOvergenomenUitKbo.StreamKey!,
                                                                     contactgegevenWerdOvergenomenUitKbo.Data.ContactgegevenId.ToString()),
                        JsonLdType.Contactgegeven.Type),
                    ContactgegevenId = contactgegevenWerdOvergenomenUitKbo.Data.ContactgegevenId,
                    Contactgegeventype = contactgegevenWerdOvergenomenUitKbo.Data.Contactgegeventype,
                    Waarde = contactgegevenWerdOvergenomenUitKbo.Data.Waarde,
                    Beschrijving = "",
                    IsPrimair = false,
                });

        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
    }
}

public class Given_ContactgegevenWerdGewijzigdUitKBO
{
    [Fact]
    public void Then_it_updates_the_contactgegeven_to_the_detail()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var contactgegevenWerdGewijzigdUitKbo = fixture.Create<TestEvent<ContactgegevenWerdGewijzigdInKbo>>();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        var contactgegevenBeforeEdit = new PubliekVerenigingDetailDocument.Types.Contactgegeven
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
        };

        doc.Contactgegevens = new[]
        {
            contactgegevenBeforeEdit,
        };

        PubliekVerenigingDetailProjector.Apply(contactgegevenWerdGewijzigdUitKbo, doc);

        doc.Contactgegevens.Should()
           .ContainSingle(c => c.ContactgegevenId == contactgegevenWerdGewijzigdUitKbo.Data.ContactgegevenId)
           .Which.Should().BeEquivalentTo(
                new PubliekVerenigingDetailDocument.Types.Contactgegeven
                {
                    JsonLdMetadata = contactgegevenBeforeEdit.JsonLdMetadata,
                    ContactgegevenId = contactgegevenBeforeEdit.ContactgegevenId,
                    Beschrijving = contactgegevenBeforeEdit.Beschrijving,
                    Contactgegeventype = contactgegevenBeforeEdit.Contactgegeventype,
                    Waarde = contactgegevenWerdGewijzigdUitKbo.Data.Waarde,
                    IsPrimair = contactgegevenBeforeEdit.IsPrimair,
                });

        doc.Contactgegevens.Should().BeInAscendingOrder(c => c.ContactgegevenId);
    }
}
