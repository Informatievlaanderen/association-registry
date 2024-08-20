<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Historiek/Projector/Given_ContactgegevenKonNietOvergenomenWordenUitKBO.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Historiek.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Historiek.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Historiek/Projecting/Given_ContactgegevenKonNietOvergenomenWordenUitKBO.cs

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
public class Given_ContactgegevenKonNietOvergenomenWordenUitKBO
{
    [Fact]
    public void Then_it_adds_a_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var contactgegevenKonNietOvergenomenWorden = fixture.Create<TestEvent<ContactgegevenKonNietOvergenomenWordenUitKBO>>();

        var doc = new BeheerVerenigingHistoriekDocument();

        BeheerVerenigingHistoriekProjector.Apply(contactgegevenKonNietOvergenomenWorden, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"Contactgegeven ‘{contactgegevenKonNietOvergenomenWorden.Data.TypeVolgensKbo}' kon niet overgenomen worden uit KBO.",
                nameof(ContactgegevenKonNietOvergenomenWordenUitKBO),
                contactgegevenKonNietOvergenomenWorden.Data,
                contactgegevenKonNietOvergenomenWorden.Initiator,
                contactgegevenKonNietOvergenomenWorden.Tijdstip.ToZuluTime()));
    }
}
