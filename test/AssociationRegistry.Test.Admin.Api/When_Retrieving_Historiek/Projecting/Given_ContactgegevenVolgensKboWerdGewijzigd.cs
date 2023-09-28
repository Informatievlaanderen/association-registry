﻿namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenUitKboWerdGewijzigd
{
    [Fact]
    public void Then_it_updates_the_contactgegeven()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var contactgegevenWerdOvergenomen = fixture.Create<TestEvent<ContactgegevenWerdOvergenomenUitKBO>>();

        var contactgegevenWerdGewijzigdVolgensKBO = new TestEvent<ContactgegevenUitKBOWerdGewijzigd>(
            fixture.Create<ContactgegevenUitKBOWerdGewijzigd>() with
            {
                ContactgegevenId = contactgegevenWerdOvergenomen.Data.ContactgegevenId,
            });

        var doc = new BeheerVerenigingHistoriekDocument();

        BeheerVerenigingHistoriekProjector.Apply(contactgegevenWerdOvergenomen, doc);
        BeheerVerenigingHistoriekProjector.Apply(contactgegevenWerdGewijzigdVolgensKBO, doc);

        doc.Gebeurtenissen.Last().Should().BeEquivalentTo(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"{contactgegevenWerdOvergenomen.Data.TypeVolgensKbo} '{contactgegevenWerdOvergenomen.Data.Waarde}' werd gewijzigd.",
                nameof(ContactgegevenUitKBOWerdGewijzigd),
                contactgegevenWerdGewijzigdVolgensKBO.Data,
                contactgegevenWerdGewijzigdVolgensKBO.Initiator,
                contactgegevenWerdGewijzigdVolgensKBO.Tijdstip.ToZuluTime()));
    }
}
