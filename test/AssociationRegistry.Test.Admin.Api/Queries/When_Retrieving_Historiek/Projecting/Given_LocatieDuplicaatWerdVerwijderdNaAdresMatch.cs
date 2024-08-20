<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Historiek/Projector/Given_LocatieDuplicaatWerdVerwijderdNaAdresMatch.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Historiek.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Historiek.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Historiek/Projecting/Given_LocatieDuplicaatWerdVerwijderdNaAdresMatch.cs

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
public class Given_LocatieDuplicaatWerdVerwijderdNaAdresMatch
{
    [Fact]
    public void Then_it_adds_the_locatie_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var locatieDuplicaatWerdVerwijderd = fixture.Create<TestEvent<LocatieDuplicaatWerdVerwijderdNaAdresMatch>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(locatieDuplicaatWerdVerwijderd, doc);

        var naam = string.IsNullOrEmpty(locatieDuplicaatWerdVerwijderd.Data.LocatieNaam)
            ? string.Empty
            : locatieDuplicaatWerdVerwijderd.Data.LocatieNaam;

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"Locatie '{naam}' met ID {locatieDuplicaatWerdVerwijderd.Data.VerwijderdeLocatieId} werd verwijderd omdat de gegevens exact overeenkomen met locatie ID {locatieDuplicaatWerdVerwijderd.Data.BehoudenLocatieId}.",
                nameof(LocatieDuplicaatWerdVerwijderdNaAdresMatch),
                locatieDuplicaatWerdVerwijderd.Data,
                locatieDuplicaatWerdVerwijderd.Initiator,
                locatieDuplicaatWerdVerwijderd.Tijdstip.ToZuluTime()));
    }
}
