<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Historiek/Projector/Given_FeitelijkeVerenigingWerdGeregistreerd.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Historiek.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Historiek.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Historiek/Projecting/Given_FeitelijkeVerenigingWerdGeregistreerd.cs

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Admin.Schema.Historiek.EventData;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_FeitelijkeVerenigingWerdGeregistreerd
{
    [Fact]
    public void Then_it_creates_a_new_document()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var feitelijkeVerenigingWerdGeregistreerd = fixture.Create<TestEvent<FeitelijkeVerenigingWerdGeregistreerd>>();

        var document = BeheerVerenigingHistoriekProjector.Create(feitelijkeVerenigingWerdGeregistreerd);

        document.Gebeurtenissen.Should().BeEquivalentTo(
            new List<BeheerVerenigingHistoriekGebeurtenis>
            {
                new(
                    $"Feitelijke vereniging werd geregistreerd met naam '{feitelijkeVerenigingWerdGeregistreerd.Data.Naam}'.",
                    nameof(FeitelijkeVerenigingWerdGeregistreerd),
                    FeitelijkeVerenigingWerdGeregistreerdData.Create(feitelijkeVerenigingWerdGeregistreerd.Data),
                    feitelijkeVerenigingWerdGeregistreerd.Initiator,
                    feitelijkeVerenigingWerdGeregistreerd.Tijdstip.ToZuluTime()),
            }
        );
    }
}
