<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Historiek/Projector/Given_ContactgegevenWerdToegevoegd.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Historiek.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Historiek.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Historiek/Projecting/Given_ContactgegevenWerdToegevoegd.cs

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_ContactgegevenWerdToegevoegd
{
    [Fact]
    public void Then_it_adds_the_contactgegeven()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var contactgegevenWerdToegevoegd = fixture.Create<TestEvent<ContactgegevenWerdToegevoegd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(contactgegevenWerdToegevoegd, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"{contactgegevenWerdToegevoegd.Data.Contactgegeventype} '{contactgegevenWerdToegevoegd.Data.Waarde}' werd toegevoegd.",
                nameof(ContactgegevenWerdToegevoegd),
                contactgegevenWerdToegevoegd.Data,
                contactgegevenWerdToegevoegd.Initiator,
                contactgegevenWerdToegevoegd.Tijdstip.ToZuluTime()));
    }
}
