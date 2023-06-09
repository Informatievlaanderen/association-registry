﻿namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Projections.Detail;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;
using Formatters = AssociationRegistry.Admin.Api.Infrastructure.Extensions.Formatters;

[UnitTest]
public class Given_ContactgegevenWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_contactgegeven()
    {
        var fixture = new Fixture().CustomizeAll();
        var contactgegevenWerdGewijzigd = fixture.Create<TestEvent<ContactgegevenWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();
        doc.Contactgegevens = new[]
        {
            new BeheerVerenigingDetailDocument.Contactgegeven
            {
                ContactgegevenId = contactgegevenWerdGewijzigd.Data.ContactgegevenId,
                Type = contactgegevenWerdGewijzigd.Data.Type,
                Waarde = fixture.Create<string>(),
                Beschrijving = fixture.Create<string>(),
                IsPrimair = true,
            },
        };

        BeheerVerenigingDetailProjector.Apply(contactgegevenWerdGewijzigd, doc);

        doc.Contactgegevens.Should().BeEquivalentTo(
            new[]
            {
                new BeheerVerenigingDetailDocument.Contactgegeven
                {
                    ContactgegevenId = contactgegevenWerdGewijzigd.Data.ContactgegevenId,
                    Type = contactgegevenWerdGewijzigd.Data.Type,
                    Waarde = contactgegevenWerdGewijzigd.Data.Waarde,
                    Beschrijving = contactgegevenWerdGewijzigd.Data.Beschrijving,
                    IsPrimair = contactgegevenWerdGewijzigd.Data.IsPrimair,
                },
            });
        doc.DatumLaatsteAanpassing.Should().Be(Formatters.ToBelgianDate(contactgegevenWerdGewijzigd.Tijdstip));
        doc.Metadata.Should().BeEquivalentTo(new Metadata(contactgegevenWerdGewijzigd.Sequence, contactgegevenWerdGewijzigd.Version));
    }
}
