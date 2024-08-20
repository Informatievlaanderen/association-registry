<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Historiek/Projector/Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Historiek.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Historiek.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Historiek/Projecting/Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.cs

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
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
{
    private readonly BeheerVerenigingHistoriekDocument _document;
    private readonly TestEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> _verenigingMetRechtspersoonlijkheidWerdGeregistreerd;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        _verenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<TestEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>>();

        _document = BeheerVerenigingHistoriekProjector.Create(_verenigingMetRechtspersoonlijkheidWerdGeregistreerd);
    }

    [Fact]
    public void Then_it_creates_a_new_document()
    {
        _document.Gebeurtenissen.Should().BeEquivalentTo(
            new List<BeheerVerenigingHistoriekGebeurtenis>
            {
                new(
                    $"Vereniging met rechtspersoonlijkheid werd geregistreerd met naam '{_verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Naam}'.",
                    nameof(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd),
                    _verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data,
                    _verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Initiator,
                    _verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Tijdstip.ToZuluTime()),
            }
        );
    }
}
