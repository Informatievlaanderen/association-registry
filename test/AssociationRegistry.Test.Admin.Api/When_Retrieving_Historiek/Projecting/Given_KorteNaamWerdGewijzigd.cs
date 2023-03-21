﻿namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.Api.Projections.Historiek;
using Events;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_KorteNaamWerdGewijzigd
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var projectEventOnHistoriekDocument =
            WhenApplying<KorteNaamWerdGewijzigd>
                .ToHistoriekProjectie();

        projectEventOnHistoriekDocument.AppendsTheCorrectGebeurtenissen(
            (i, t) => new BeheerVerenigingHistoriekGebeurtenis($"Korte naam werd gewijzigd naar '{projectEventOnHistoriekDocument.Event.Data.KorteNaam}'.", nameof(KorteNaamWerdGewijzigd), i, t));
    }
}
